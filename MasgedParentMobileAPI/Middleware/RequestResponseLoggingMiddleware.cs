using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using MasgedParentMobileAPI.Configuration;
using MasgedParentMobileAPI.Models;
using MasgedTeacherMobileAPI.Data;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace MasgedParentMobileAPI.Middleware;

public sealed class RequestResponseLoggingMiddleware(
    RequestDelegate next,
    IOptions<RequestLoggingSettings> settings,
    IServiceScopeFactory scopeFactory,
    ILogger<RequestResponseLoggingMiddleware> logger)
{
    /// <summary>Redacts quoted JSON string values for keys password, PasswordPlain, otp, token.</summary>
    private static readonly Regex SensitiveJsonQuotedRegex = new(
        "(\"(?:password|passwordplain|otp|token)\"\\s*:\\s*\")(?:[^\"\\\\]|\\\\.)*(\")",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly JsonSerializerOptions JsonHeaderOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = false };

    public async Task InvokeAsync(HttpContext context)
    {
        var opts = settings.Value ?? new RequestLoggingSettings();
        if (!opts.Enabled || ShouldSkip(context, opts))
        {
            await next(context);
            return;
        }

        await LogRequestAsync(context, opts);
    }

    private static bool ShouldSkip(HttpContext context, RequestLoggingSettings opts)
    {
        var pathValue = context.Request.Path.Value ?? string.Empty;
        foreach (var prefix in opts.ExcludedPaths ?? Array.Empty<string>())
        {
            if (string.IsNullOrWhiteSpace(prefix))
                continue;

            var p = prefix.Trim();
            if (!p.StartsWith("/", StringComparison.Ordinal))
                p = "/" + p;

            if (pathValue.StartsWith(p, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private async Task LogRequestAsync(HttpContext context, RequestLoggingSettings opts)
    {
        var requestedAtUtc = DateTime.UtcNow;

        context.Request.EnableBuffering();
        string rawRequestBody;
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            rawRequestBody = await reader.ReadToEndAsync(context.RequestAborted);
            context.Request.Body.Position = 0;
        }

        var requestHeadersJson = SerializeHeaders(context.Request.Headers, opts.MaxBodyLength);

        var originalResponseBody = context.Response.Body;
        await using var responseBuf = new MemoryStream();
        context.Response.Body = responseBuf;

        var stopwatch = Stopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
        }

        responseBuf.Position = 0;
        var rawResponseBody = await new StreamReader(responseBuf, Encoding.UTF8, leaveOpen: true).ReadToEndAsync(context.RequestAborted);
        responseBuf.Position = 0;
        await responseBuf.CopyToAsync(originalResponseBody, context.RequestAborted);
        context.Response.Body = originalResponseBody;

        try
        {
            await PersistLogAsync(context, opts, requestedAtUtc, rawRequestBody, rawResponseBody, requestHeadersJson, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist API request log.");
        }
    }

    private async Task PersistLogAsync(
        HttpContext context,
        RequestLoggingSettings opts,
        DateTime requestedAtUtc,
        string rawRequestBody,
        string rawResponseBody,
        string requestHeadersJson,
        long elapsedMs)
    {
        var user = context.User;
        var preparedRequestBody = PrepareText(rawRequestBody, opts.MaxBodyLength);
        var preparedResponseBody = PrepareText(rawResponseBody, opts.MaxBodyLength);

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entry = new ApiRequestLog
        {
            RequestedAt = requestedAtUtc,
            Method = context.Request.Method,
            Path = TruncateHard(context.Request.Path.Value ?? "", 500),
            QueryString = string.IsNullOrEmpty(context.Request.QueryString.Value)
                ? null
                : TruncateHard(context.Request.QueryString.Value!, 2000),
            RequestHeaders = TruncateHard(requestHeadersJson, opts.MaxBodyLength * 2),
            RequestBody = preparedRequestBody,
            ResponseStatusCode = context.Response.StatusCode,
            ResponseBody = preparedResponseBody,
            DurationMs = (int)Math.Min(int.MaxValue, elapsedMs),
            ClientIp = context.Connection.RemoteIpAddress?.ToString(),
            UserId = FirstClaimValue(user, "id", ClaimTypes.NameIdentifier, JwtRegisteredClaimNames.Sub),
            UserName = FirstClaimValue(user, "fatherName", "name", ClaimTypes.Name)
        };

        db.ApiRequestLogs.Add(entry);
        await db.SaveChangesAsync(context.RequestAborted).ConfigureAwait(false);
    }

    private static string? PrepareText(string? raw, int maxBodyLength)
    {
        if (string.IsNullOrEmpty(raw))
            return null;

        var redacted = SensitiveJsonQuotedRegex.Replace(raw, "$1[REDACTED]$2");
        if (maxBodyLength <= 0 || redacted.Length <= maxBodyLength)
            return redacted;

        return redacted[..maxBodyLength] + "...[truncated]";
    }

    private static string TruncateHard(string value, int maxLen)
        => maxLen <= 0 || value.Length <= maxLen ? value : value[..maxLen] + "...[truncated]";

    private static string SerializeHeaders(IHeaderDictionary headers, int maxRough)
    {
        var pairs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var h in headers)
        {
            var value = string.Equals(h.Key, HeaderNames.Authorization, StringComparison.OrdinalIgnoreCase)
                ? "[REDACTED]"
                : string.Join(", ", h.Value.ToArray());

            pairs[h.Key] = value;
        }

        var json = JsonSerializer.Serialize(pairs, JsonHeaderOptions);
        if (json.Length > maxRough * 4 && maxRough > 0)
        {
            var take = Math.Min(json.Length, maxRough * 4);
            return json[..take] + "...";
        }

        return json;
    }

    private static string? FirstClaimValue(ClaimsPrincipal? user, params string[] claimTypes)
    {
        foreach (var t in claimTypes)
        {
            var claim = user?.FindFirst(t);
            var v = claim?.Value?.Trim();
            if (!string.IsNullOrEmpty(v))
                return v;
        }

        return null;
    }
}
