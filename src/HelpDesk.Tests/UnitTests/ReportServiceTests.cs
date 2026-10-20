using HelpDesk.BusinessLogic.Services;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Enums;
using Moq;
using Xunit;

namespace HelpDesk.UnitTests.Services;

public class ReportServiceTests
{
    private readonly Mock<ITicketRepository> _ticketRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IStatusRepository> _statusRepoMock;
    private readonly Mock<IPriorityRepository> _priorityRepoMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly ReportService _service;

    public ReportServiceTests()
    {
        _ticketRepoMock = new Mock<ITicketRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _statusRepoMock = new Mock<IStatusRepository>();
        _priorityRepoMock = new Mock<IPriorityRepository>();
        _categoryRepoMock = new Mock<ICategoryRepository>();

        _service = new ReportService(
            _ticketRepoMock.Object,
            _userRepoMock.Object,
            _statusRepoMock.Object,
            _priorityRepoMock.Object,
            _categoryRepoMock.Object);
    }

    [Fact]
    public async Task GetTechnicianWorkloadAsync_ShouldReturnWorkload_WhenActiveTechniciansAndValidTickets()
    {
        // Arrange
        var tecnicoId = Guid.NewGuid();
        var tecnico = new Tecnico
        {
            Id = tecnicoId,
            Email = "tecnico@test.com",
            NombreCompleto = "Carlos Tecnico",
            Rol = UserRole.Tecnico,
            Activo = true
        };

        var prioridadMedia = new Priority { Id = Guid.NewGuid(), Nombre = "Media", Nivel = 2, SLAHoras = 24 };
        var estadoEnProgreso = new Status { Id = Guid.NewGuid(), Nombre = "En Progreso", EsFinal = false };
        var estadoResuelto = new Status { Id = Guid.NewGuid(), Nombre = "Resuelto", EsFinal = true };

        var ticketActivo = new Ticket
        {
            Id = Guid.NewGuid(),
            TecnicoId = tecnicoId,
            Prioridad = prioridadMedia,
            Estado = estadoEnProgreso,
            FechaCreacion = DateTime.UtcNow.AddHours(-1)
        };

        var ticketResuelto = new Ticket
        {
            Id = Guid.NewGuid(),
            TecnicoId = tecnicoId,
            Prioridad = prioridadMedia,
            Estado = estadoResuelto,
            FechaCreacion = DateTime.UtcNow.AddHours(-5),
            FechaResolucion = DateTime.UtcNow.AddHours(-1)
        };

        _userRepoMock.Setup(x => x.GetByRoleAsync(UserRole.Tecnico.ToString(), true))
            .ReturnsAsync(new List<User> { tecnico });

        _ticketRepoMock.Setup(x => x.GetAllAsync(true))
            .ReturnsAsync(new List<Ticket> { ticketActivo, ticketResuelto });

        // Act
        var result = (await _service.GetTechnicianWorkloadAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var workload = result.First();
        Assert.Equal(tecnicoId, workload.Tecnico.Id);
        Assert.Equal(1, workload.ActiveTickets);
        Assert.Equal(1, workload.TicketsResolvedThisMonth);
        Assert.Equal(WorkloadLevel.Medio, workload.WorkloadLevel);
    }

    [Fact]
    public async Task GetTicketStatisticsAsync_ShouldReturnStats_WhenUserIsSupervisor()
    {
        // Arrange
        var expectedDto = new TicketStatisticsDto(
            TotalTickets: 10,
            TicketsAbiertos: 3,
            TicketsEnProceso: 2,
            TicketsResueltos: 4,
            TicketsCerrados: 1,
            PromedioTiempoResolucionHoras: 2.5,
            TicketsPorTecnico: new Dictionary<string, int>(),
            PromedioSatisfaccion: 4.8
        );

        _ticketRepoMock.Setup(x => x.GetStatisticsAsync(null, null))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetTicketStatisticsAsync(null, null, "Supervisor");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.TotalTickets);
        Assert.Equal(2.5, result.PromedioTiempoResolucionHoras);
        _ticketRepoMock.Verify(x => x.GetStatisticsAsync(null, null), Times.Once);
    }

    [Fact]
    public async Task GetTicketStatisticsAsync_ShouldThrow_WhenUserIsNotSupervisor()
    {
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.GetTicketStatisticsAsync(null, null, "Empleado"));

        _ticketRepoMock.Verify(x => x.GetStatisticsAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Never);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_ShouldCalculateStats_WhenTicketsExist()
    {
        // Arrange
        var prioridad = new Priority { Id = Guid.NewGuid(), Nombre = "Media", Nivel = 2, SLAHoras = 24 };
        var estadoAbierto = new Status { Id = Guid.NewGuid(), Nombre = "Abierto", EsFinal = false };
        var estadoCerrado = new Status { Id = Guid.NewGuid(), Nombre = "Cerrado", EsFinal = true };

        var ticket1 = new Ticket
        {
            Id = Guid.NewGuid(),
            Prioridad = prioridad,
            Estado = estadoAbierto,
            FechaCreacion = DateTime.UtcNow
        };

        var ticket2 = new Ticket
        {
            Id = Guid.NewGuid(),
            Prioridad = prioridad,
            Estado = estadoCerrado,
            FechaCreacion = DateTime.UtcNow.AddHours(-2),
            FechaResolucion = DateTime.UtcNow
        };

        _ticketRepoMock.Setup(x => x.GetAllAsync(true))
            .ReturnsAsync(new List<Ticket> { ticket1, ticket2 });

        // Act
        var result = await _service.GetDashboardStatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalTickets);
        Assert.Equal(1, result.OpenTickets);
        Assert.Equal(1, result.ClosedTickets);
    }
}
