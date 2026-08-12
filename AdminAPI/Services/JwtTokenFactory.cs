using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AdminAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace AdminAPI.Services;

public class JwtTokenFactory(IConfiguration configuration)
{
    public string CreateToken(Teacher teacher, string username)
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = configuration["Jwt:Issuer"] ?? "AdminAPI";
        var audience = configuration["Jwt:Audience"] ?? "AdminPanelUI";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, teacher.Id.ToString()),
            new(ClaimTypes.Name, username),
            new("IsAdmin", teacher.UsersManage.ToString()),
            new("IsGirlTeacher", (teacher.IsGirlTeacher ?? false).ToString()),
            new("IsViewOnly", teacher.IsViewOnly.ToString()),
            new("IsSupervisor", teacher.IsSupervisor.ToString()),
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: KuwaitTime.Now.AddMonths(12),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
