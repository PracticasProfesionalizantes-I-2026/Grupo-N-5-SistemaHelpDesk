using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HelpDesk.BusinessLogic.Services;

/// <summary>
/// Servicio responsable de generar tokens JWT firmados para la autenticación de usuarios.
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var secretKey = _configuration["Jwt:Key"] ?? "HelpDeskSecretKeyParaFirmaDeTokenJwtSeguro2026!";
        var issuer = _configuration["Jwt:Issuer"] ?? "HelpDeskAPI";
        var audience = _configuration["Jwt:Audience"] ?? "HelpDeskClient";
        var durationMinutes = int.TryParse(_configuration["Jwt:DurationInMinutes"], out var d) ? d : 480;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new("id", user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Name, user.NombreCompleto),
            new(ClaimTypes.Role, user.Rol.ToString()),
            new("role", user.Rol.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(durationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
