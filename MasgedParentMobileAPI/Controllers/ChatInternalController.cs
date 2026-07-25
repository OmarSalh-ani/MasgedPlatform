using System.Security.Cryptography;
using System.Text;
using MasgedParentMobileAPI.Configuration;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MasgedParentMobileAPI.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/chat/internal")]
public sealed class ChatInternalController(
    IOptions<ChatInternalSettings> chatOptions,
    IChatRealtimePublisher publisher)
    : ControllerBase
{
    public const string InternalKeyHeaderName = "X-Chat-Internal-Key";

    /// <summary>Called by Teacher API after persisting teacher→parent chat (same DB).</summary>
    [HttpPost("broadcast-message")]
    public async Task<IActionResult> BroadcastReceiveMessage(
        [FromBody] ChatMessageDto message,
        CancellationToken cancellationToken)
    {
        if (!ValidateKey())
            return Unauthorized();

        if (message == null ||
            message.TeacherId <= 0 ||
            message.StudentId is null or <= 0 ||
            string.IsNullOrWhiteSpace(message.ParentPhone) ||
            string.IsNullOrWhiteSpace(message.MessageText))
            return BadRequest();

        var canonicalPhone = PhoneNormalizer.ToCanonical(message.ParentPhone);
        await publisher.PublishReceiveMessage(new ChatMessageDto
        {
            Id = message.Id,
            ParentPhone = canonicalPhone,
            TeacherId = message.TeacherId,
            SenderType = message.SenderType,
            MessageText = message.MessageText,
            StudentId = message.StudentId,
            SentAt = message.SentAt,
        }, cancellationToken);

        return Ok();
    }

    private bool ValidateKey()
    {
        Request.Headers.TryGetValue(InternalKeyHeaderName, out var hdr);
        var expected = chatOptions.Value.InternalBroadcastKey ?? string.Empty;
        if (string.IsNullOrWhiteSpace(expected) || string.IsNullOrWhiteSpace(hdr))
            return false;
        var provided = hdr.ToString();
        return FixedTimeEquals(provided, expected);
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        var ab = Encoding.UTF8.GetBytes(a);
        var bb = Encoding.UTF8.GetBytes(b);
        return ab.Length == bb.Length && CryptographicOperations.FixedTimeEquals(ab, bb);
    }
}
