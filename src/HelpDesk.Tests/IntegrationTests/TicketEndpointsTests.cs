using System.Net;
using System.Net.Http.Json;
using HelpDesk.Shared.DTOs;
using Xunit;

namespace HelpDesk.Tests.IntegrationTests;

public class TicketEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TicketEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTicketAndGetById_ShouldReturn201And200()
    {
        // 1. Arrange - Headers de Empleado (Juan Pérez del seed data)
        var empleadoId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee";
        var prioridadId = Guid.Parse("22222222-2222-2222-2222-222222222222"); // Media
        var categoriaId = Guid.Parse("55555555-5555-5555-5555-555555555555"); // Hardware

        var createDto = new TicketCreateDTO(
            "Ticket de prueba de integración",
            "Fallo en equipo de oficina para testing de integración",
            prioridadId,
            categoriaId);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/tickets")
        {
            Content = JsonContent.Create(createDto)
        };
        request.Headers.Add("X-User-Id", empleadoId);
        request.Headers.Add("X-User-Role", "Empleado");

        // 2. Act - POST /api/tickets
        var createResponse = await _client.SendAsync(request);

        // 3. Assert - 201 Created
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdTicket = await createResponse.Content.ReadFromJsonAsync<TicketResponseDTO>();
        Assert.NotNull(createdTicket);
        Assert.Equal(createDto.Titulo, createdTicket.Titulo);
        Assert.NotEqual(Guid.Empty, createdTicket.Id);

        // 4. Act - GET /api/tickets/{id}
        using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/tickets/{createdTicket.Id}");
        getRequest.Headers.Add("X-User-Id", empleadoId);
        getRequest.Headers.Add("X-User-Role", "Empleado");

        var getResponse = await _client.SendAsync(getRequest);

        // 5. Assert - 200 OK
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetchedTicket = await getResponse.Content.ReadFromJsonAsync<TicketResponseDTO>();
        Assert.NotNull(fetchedTicket);
        Assert.Equal(createdTicket.Id, fetchedTicket.Id);
        Assert.Equal("Ticket de prueba de integración", fetchedTicket.Titulo);
    }

    [Fact]
    public async Task GetTicketById_ShouldReturnNotFound_WhenTicketDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/tickets/{nonExistentId}");
        request.Headers.Add("X-User-Role", "Supervisor");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("{\"tecnicoId\": \"123\"}")]
    [InlineData("{\"tecnicoId\": \"\"}")]
    [InlineData("{\"tecnicoId\": 123}")]
    public async Task AssignTechnician_ShouldReturn400WithSpanishGuidMessage_WhenTecnicoIdIsInvalidStringOrEmpty(string invalidJson)
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        using var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/tickets/{ticketId}/assign")
        {
            Content = new StringContent(invalidJson, System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Add("X-User-Role", "Supervisor");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("El formato del identificador no es válido", body);
    }

    [Fact]
    public async Task AssignTechnician_ShouldReturn400WithSpanishEmptyBodyMessage_WhenBodyIsEmpty()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        using var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/tickets/{ticketId}/assign")
        {
            Content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Add("X-User-Role", "Supervisor");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("El cuerpo de la solicitud no puede estar vacío", body);
    }

    [Fact]
    public async Task CreateTicket_ShouldReturn400WithSpanishStringMessage_WhenNumberSentForString()
    {
        // Arrange
        var invalidJson = "{\"titulo\": 12345, \"descripcion\": \"Descripción de prueba\", \"prioridadId\": \"22222222-2222-2222-2222-222222222222\", \"categoriaId\": \"55555555-5555-5555-5555-555555555555\"}";
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/tickets")
        {
            Content = new StringContent(invalidJson, System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Add("X-User-Role", "Empleado");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("El valor del campo debe ser una cadena de texto (string)", body);
    }

    [Fact]
    public async Task CreateTicket_ShouldReturn400WithSpanishGuidMessage_WhenInvalidGuidInPrioridadId()
    {
        // Arrange
        var invalidJson = "{\"titulo\": \"Ticket prueba\", \"descripcion\": \"Descripción de prueba\", \"prioridadId\": \"123\", \"categoriaId\": \"55555555-5555-5555-5555-555555555555\"}";
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/tickets")
        {
            Content = new StringContent(invalidJson, System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Add("X-User-Role", "Empleado");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("El formato del identificador no es válido", body);
    }
}
