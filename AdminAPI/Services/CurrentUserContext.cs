using System.Security.Claims;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class CurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    public int TeacherId =>
        int.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : 0;

    public bool IsGirlTeacher =>
        bool.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue("IsGirlTeacher"), out var isGirl)
        && isGirl;

    public bool IsAdmin =>
        bool.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue("IsAdmin"), out var isAdmin)
        && isAdmin;

    public bool CanModify =>
        !bool.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue("IsViewOnly"), out var viewOnly)
        || !viewOnly;
}
