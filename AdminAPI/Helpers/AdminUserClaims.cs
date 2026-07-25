using System.Security.Claims;

namespace AdminAPI.Helpers;

public static class AdminUserClaims
{
    public static bool IsViewOnly(ClaimsPrincipal user) =>
        string.Equals(user.FindFirstValue("IsViewOnly"), "True", StringComparison.OrdinalIgnoreCase);
}
