using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MasgedParentMobileAPI.Configuration;

public class JwtTokenService
{
    private readonly JwtSettings _jwt;

    public JwtTokenService(IOptions<ApiSettings> options)
    {
        _jwt = options.Value.Jwt;
    }

    public string GenerateToken(int parentId, string fatherPhone, string fatherName)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, parentId.ToString()),
            new Claim("fatherPhone", fatherPhone),
            new Claim("fatherName", fatherName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpireMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
