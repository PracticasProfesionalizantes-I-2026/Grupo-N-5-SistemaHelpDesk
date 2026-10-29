using System.Net;
using System.Net.Http.Json;
using HelpDesk.Shared.DTOs;
using Xunit;

namespace HelpDesk.Tests.IntegrationTests;

public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginDTO("admin@helpdesk.com", "Password123!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
        Assert.NotNull(result);
        Assert.Equal("admin@helpdesk.com", result.Usuario.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }

    [Fact]
    public async Task LoginAndUseBearerToken_ShouldAuthenticateAndAllowTicketCreation()
    {
        // 1. Iniciar sesión como Empleado (Juan Pérez)
        var loginDto = new LoginDTO("juan.perez@empresa.com", "Password123!");
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDTO>();
        Assert.NotNull(loginResult);
        Assert.False(string.IsNullOrWhiteSpace(loginResult.Token));

        // 2. Usar el Bearer Token para crear un ticket (sin headers X-User-Id / X-User-Role)
        var prioridadId = Guid.Parse("22222222-2222-2222-2222-222222222222"); // Media
        var categoriaId = Guid.Parse("55555555-5555-5555-5555-555555555555"); // Hardware
        var createDto = new TicketCreateDTO(
            "Ticket autenticado con Bearer Token",
            "Probando creación con token JWT obtenido del login",
            prioridadId,
            categoriaId);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/tickets")
        {
            Content = JsonContent.Create(createDto)
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

        var ticketResponse = await _client.SendAsync(request);

        // 3. Verificar que se creó exitosamente con el ID de Juan Pérez extraído del JWT
        Assert.Equal(HttpStatusCode.Created, ticketResponse.StatusCode);
        var createdTicket = await ticketResponse.Content.ReadFromJsonAsync<TicketResponseDTO>();
        Assert.NotNull(createdTicket);
        Assert.Equal(loginResult.Usuario.Id, createdTicket.Empleado.Id);
        Assert.Equal(createDto.Titulo, createdTicket.Titulo);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
    {
        // Arrange
        var loginDto = new LoginDTO("admin@helpdesk.com", "PasswordIncorrecta!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var loginDto = new LoginDTO("noexiste@empresa.com", "Password123!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
