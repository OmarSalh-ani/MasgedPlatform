namespace MasgedTeacherMobileAPI.Dtos;

public class GlobalResponse
{
    public bool Success { get; set; }
    public ResponseStatusCode StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }

    public static GlobalResponse Ok(object? data = null, string? message = null) =>
        new()
        {
            Success = true,
            StatusCode = ResponseStatusCode.Ok,
            Message = message ?? DefaultMessages.Ok,
            Data = data
        };

    public static GlobalResponse Ok<T>(T? data, string? message = null) =>
        Ok((object?)data, message);

    public static GlobalResponse Created(object? data = null, string? message = null) =>
        new()
        {
            Success = true,
            StatusCode = ResponseStatusCode.Created,
            Message = message ?? DefaultMessages.Created,
            Data = data
        };

    public static GlobalResponse Fail(
        ResponseStatusCode statusCode,
        string message,
        object? data = null) =>
        new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };

    public static GlobalResponse BadRequest(string? message = null, object? data = null) =>
        Fail(ResponseStatusCode.BadRequest, message ?? DefaultMessages.BadRequest, data);

    public static GlobalResponse Unauthorized(string? message = null, object? data = null) =>
        Fail(ResponseStatusCode.Unauthorized, message ?? DefaultMessages.Unauthorized, data);

    public static GlobalResponse Forbidden(string? message = null, object? data = null) =>
        Fail(ResponseStatusCode.Forbidden, message ?? DefaultMessages.Forbidden, data);

    public static GlobalResponse NotFound(string? message = null, object? data = null) =>
        Fail(ResponseStatusCode.NotFound, message ?? DefaultMessages.NotFound, data);

    public static GlobalResponse Conflict(string? message = null, object? data = null) =>
        Fail(ResponseStatusCode.Conflict, message ?? DefaultMessages.Conflict, data);

    public static GlobalResponse InternalServerError(string? message = null, object? data = null) =>
        Fail(ResponseStatusCode.InternalServerError, message ?? DefaultMessages.InternalServerError, data);

    public static class DefaultMessages
    {
        public const string Ok = "تمت العملية بنجاح";
        public const string Created = "تم الإنشاء بنجاح";
        public const string BadRequest = "طلب غير صالح";
        public const string Unauthorized = "غير مصرح بالوصول";
        public const string Forbidden = "غير مسموح بهذا الإجراء";
        public const string NotFound = "البيانات غير موجودة";
        public const string Conflict = "تعارض في البيانات";
        public const string InternalServerError = "حدث خطأ في الخادم";
    }
}
