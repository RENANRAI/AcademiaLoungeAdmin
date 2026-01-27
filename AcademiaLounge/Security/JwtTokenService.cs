using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AcademiaLounge.Models;
using Microsoft.IdentityModel.Tokens;

namespace AcademiaLounge.Security;

public class JwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config) => _config = config;

    public (string token, DateTimeOffset expiresAt) Generate(Usuario user)
    {
        var jwt = _config.GetSection("Jwt");
        var key = (jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key não configurado.")).Trim();
        var issuer = (jwt["Issuer"] ?? "academia").Trim();
        var audience = (jwt["Audience"] ?? "academia").Trim();
        var minutes = int.TryParse(jwt["Minutes"], out var m) ? m : 120;

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(minutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Login),
            new(ClaimTypes.Name, user.Nome),
            new(ClaimTypes.Role, user.Perfil.ToString())
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
