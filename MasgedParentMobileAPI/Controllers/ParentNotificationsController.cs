using System.Security.Claims;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedParentMobileAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Controllers;

[ApiController]
[Route("api/parent/[controller]")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly NewMasgedTeacherAPIDBContext _db;

    public NotificationsController(NewMasgedTeacherAPIDBContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<ParentNotificationDto>>> Get(CancellationToken cancellationToken)
    {
        var fp = User.FindFirstValue("fatherPhone");
        if (string.IsNullOrEmpty(fp)) return Unauthorized();
        var variants = PhoneNormalizer.GetVariants(fp).ToList();

        var students = await _db.RegisterForms
            .Where(r => variants.Contains(r.FatherPhone) || variants.Contains(r.FatherPhone2))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        var studentIds = students.ToHashSet();

        var list = new List<ParentNotificationDto>();

        var cutoff = DateTime.UtcNow.Date.AddDays(-30);

        var news = await _db.NewsItems
            .AsNoTracking()
            .Where(n => n.NewsDate >= cutoff || n.CreatedAt >= cutoff)
            .OrderByDescending(n => n.SortOrder)
            .ThenByDescending(n => n.NewsDate)
            .Take(40)
            .ToListAsync(cancellationToken);

        foreach (var n in news)
        {
            var created = n.NewsDate;
            list.Add(new ParentNotificationDto
            {
                Kind = "news",
                Id = n.Id,
                Title = n.Title ?? "خبر",
                Summary =
                    string.IsNullOrWhiteSpace(n.Description)
                        ? string.Empty
                        : (n.Description.Length > 200 ? n.Description[..200] + "…" : n.Description),
                CreatedAt = created,
            });
        }

        var rawMeetings = await _db.MeetingsInfos
            .AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .Take(80)
            .ToListAsync(cancellationToken);

        foreach (var m in rawMeetings)
        {
            if (studentIds.Count == 0 || string.IsNullOrWhiteSpace(m.StudentIds)) continue;

            foreach (var part in m.StudentIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (int.TryParse(part, out var sid) && studentIds.Contains(sid))
                {
                    var isActive = m.Status == 0;
                    var endedAt = m.EndedAt;
                    list.Add(new ParentNotificationDto
                    {
                        Kind = "meet",
                        Id = m.Id,
                        Title = string.IsNullOrWhiteSpace(m.MeetingName) ? "مكالمة فيديو" : m.MeetingName!,
                        Summary = isActive
                            ? $"{(m.StartDateTime ?? m.CreatedAt):yyyy/MM/dd HH:mm} — اضغط للانضمام"
                            : $"انتهت المكالمة — {(endedAt ?? m.CreatedAt):yyyy/MM/dd HH:mm}",
                        CreatedAt = m.CreatedAt == default ? (m.StartDateTime ?? DateTime.UtcNow) : m.CreatedAt,
                        CanJoin = isActive,
                        EndedAt = endedAt,
                    });
                    break;
                }
            }
        }

        list.Sort((a, b) => b.CreatedAt.CompareTo(a.CreatedAt));
        return Ok(list.Take(100).ToList());
    }
}
