using Microsoft.AspNetCore.Builder;

namespace MasgedParentMobileAPI.Middleware;

public static class RequestResponseLoggingExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        => app.UseMiddleware<RequestResponseLoggingMiddleware>();
}
