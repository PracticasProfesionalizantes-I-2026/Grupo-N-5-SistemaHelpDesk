using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HelpDesk.BusinessLogic.Services;
using HelpDesk.DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HelpDesk.Tests.UnitTests;

public class TokenServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "HelpDeskSecretKeyParaFirmaDeTokenJwtSeguro2026!" },
            { "Jwt:Issuer", "HelpDeskAPI" },
            { "Jwt:Audience", "HelpDeskClient" },
            { "Jwt:DurationInMinutes", "60" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _tokenService = new TokenService(_configuration);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwt_WithExpectedClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Empleado
        {
            Id = userId,
            Email = "empleado@test.com",
            NombreCompleto = "Juan Perez",
            Activo = true
        };

        // Act
        var tokenString = _tokenService.GenerateToken(user);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(tokenString));

        var handler = new JwtSecurityTokenHandler();
        var canRead = handler.CanReadToken(tokenString);
        Assert.True(canRead);

        var jwt = handler.ReadJwtToken(tokenString);
        Assert.Equal("HelpDeskAPI", jwt.Issuer);
        Assert.Contains("HelpDeskClient", jwt.Audiences);

        // Check claims
        var subClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier);
        Assert.NotNull(subClaim);
        Assert.Equal(userId.ToString(), subClaim.Value);

        var emailClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email || c.Type == ClaimTypes.Email);
        Assert.NotNull(emailClaim);
        Assert.Equal("empleado@test.com", emailClaim.Value);

        var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
        Assert.NotNull(roleClaim);
        Assert.Equal("Empleado", roleClaim.Value);
    }
}
