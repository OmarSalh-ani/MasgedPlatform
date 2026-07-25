using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MasgedTeacherMobileAPI-SecretKey-ChangeInProduction-Min32Chars!"));
var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
var claims = new[]
{
    new Claim("id", "17"),
    new Claim("circleId", "1"),
    new Claim(JwtRegisteredClaimNames.Sub, "17"),
};
var token = new JwtSecurityToken("MasgedTeacherMobileAPI", "MasgedTeacherMobileApp", claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);
Console.WriteLine(new JwtSecurityTokenHandler().WriteToken(token));
