using System.Net;
using System.Text.Json;
using AdminAPI.DTOs.Common;
using AdminAPI.Exceptions;
using FluentValidation;

namespace AdminAPI.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, ex);
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message, errors) = ex switch
        {
            ValidationException validation => (
                HttpStatusCode.BadRequest,
                "Validation failed",
                validation.Errors.Select(e => e.ErrorMessage).ToList()),
            KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message, new List<string>()),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, ex.Message, new List<string>()),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred", new List<string>())
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiResponseDto<object>
        {
            Success = false,
            Message = message,
            Errors = errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
