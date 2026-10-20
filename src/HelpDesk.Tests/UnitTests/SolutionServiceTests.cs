using HelpDesk.BusinessLogic.Services;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Exceptions;
using Moq;
using Xunit;

namespace HelpDesk.UnitTests.Services;

public class SolutionServiceTests
{
    private readonly Mock<ITicketRepository> _ticketRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ISolutionRepository> _solutionRepoMock;
    private readonly Mock<IStatusHistoryRepository> _statusHistoryRepoMock;
    private readonly Mock<IStatusRepository> _statusRepoMock;
    private readonly SolutionService _service;

    public SolutionServiceTests()
    {
        _ticketRepoMock = new Mock<ITicketRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _solutionRepoMock = new Mock<ISolutionRepository>();
        _statusHistoryRepoMock = new Mock<IStatusHistoryRepository>();
        _statusRepoMock = new Mock<IStatusRepository>();

        _service = new SolutionService(
            _ticketRepoMock.Object,
            _userRepoMock.Object,
            _solutionRepoMock.Object,
            _statusHistoryRepoMock.Object,
            _statusRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldAttachSolution_WhenAssignedTechnicianAndOpenTicket()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var tecnicoId = Guid.NewGuid();
        var estadoId = Guid.NewGuid();

        var tecnico = new Tecnico
        {
            Id = tecnicoId,
            Email = "tecnico@test.com",
            NombreCompleto = "Tecnico Uno",
            Rol = UserRole.Tecnico,
            Activo = true
        };

        var ticket = new Ticket
        {
            Id = ticketId,
            TecnicoId = tecnicoId,
            EstadoId = estadoId
        };

        var status = new Status { Id = estadoId, Nombre = "En Progreso", EsFinal = false };
        var dto = new SolutionCreateDTO("Se reemplazó la placa de red defectuosa.");

        _userRepoMock.Setup(x => x.GetByIdAsync(tecnicoId, It.IsAny<bool>())).ReturnsAsync(tecnico);
        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, It.IsAny<bool>())).ReturnsAsync(ticket);
        _statusRepoMock.Setup(x => x.GetByIdAsync(estadoId, It.IsAny<bool>())).ReturnsAsync(status);

        Solution? capturedSolution = null;
        _solutionRepoMock.Setup(x => x.CreateAsync(It.IsAny<Solution>()))
            .Callback<Solution>(s => capturedSolution = s)
            .ReturnsAsync((Solution s) =>
            {
                s.Id = Guid.NewGuid();
                return s;
            });

        _statusHistoryRepoMock.Setup(x => x.CreateAsync(It.IsAny<StatusHistory>()))
            .ReturnsAsync((StatusHistory h) => h);
        _ticketRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket t) => t);

        // Act
        var result = await _service.CreateAsync(ticketId, dto, tecnicoId, UserRole.Tecnico.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Descripcion, result.Description);
        Assert.Equal(ticketId, result.TicketId);
        Assert.Equal(tecnicoId, result.TecnicoId);
        _solutionRepoMock.Verify(x => x.CreateAsync(It.IsAny<Solution>()), Times.Once);
        _statusHistoryRepoMock.Verify(x => x.CreateAsync(It.IsAny<StatusHistory>()), Times.Once);
        _ticketRepoMock.Verify(x => x.UpdateAsync(ticket), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenUserIsNotTecnicoRole()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var empleadoId = Guid.NewGuid();
        var dto = new SolutionCreateDTO("Intento de solución");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedActionException>(
            () => _service.CreateAsync(ticketId, dto, empleadoId, UserRole.Empleado.ToString()));

        _solutionRepoMock.Verify(x => x.CreateAsync(It.IsAny<Solution>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenTechnicianNotAssignedToTicket()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var tecnicoId = Guid.NewGuid();
        var otroTecnicoId = Guid.NewGuid();
        var dto = new SolutionCreateDTO("Solución por técnico no asignado");

        var tecnico = new Tecnico { Id = tecnicoId, Activo = true, Rol = UserRole.Tecnico };
        var ticket = new Ticket
        {
            Id = ticketId,
            TecnicoId = otroTecnicoId, // Asignado a otro
            EstadoId = Guid.NewGuid()
        };

        _userRepoMock.Setup(x => x.GetByIdAsync(tecnicoId, It.IsAny<bool>())).ReturnsAsync(tecnico);
        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, It.IsAny<bool>())).ReturnsAsync(ticket);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedActionException>(
            () => _service.CreateAsync(ticketId, dto, tecnicoId, UserRole.Tecnico.ToString()));

        _solutionRepoMock.Verify(x => x.CreateAsync(It.IsAny<Solution>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenTicketClosed()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var tecnicoId = Guid.NewGuid();
        var estadoCerradoId = Guid.NewGuid();
        var dto = new SolutionCreateDTO("Solución en ticket cerrado");

        var tecnico = new Tecnico { Id = tecnicoId, Activo = true, Rol = UserRole.Tecnico };
        var ticket = new Ticket
        {
            Id = ticketId,
            TecnicoId = tecnicoId,
            EstadoId = estadoCerradoId
        };
        var status = new Status { Id = estadoCerradoId, Nombre = "Cerrado", EsFinal = true };

        _userRepoMock.Setup(x => x.GetByIdAsync(tecnicoId, It.IsAny<bool>())).ReturnsAsync(tecnico);
        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, It.IsAny<bool>())).ReturnsAsync(ticket);
        _statusRepoMock.Setup(x => x.GetByIdAsync(estadoCerradoId, It.IsAny<bool>())).ReturnsAsync(status);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.CreateAsync(ticketId, dto, tecnicoId, UserRole.Tecnico.ToString()));

        _solutionRepoMock.Verify(x => x.CreateAsync(It.IsAny<Solution>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowNotFound_WhenTicketDoesNotExist()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var tecnicoId = Guid.NewGuid();
        var dto = new SolutionCreateDTO("Solución");

        var tecnico = new Tecnico { Id = tecnicoId, Activo = true, Rol = UserRole.Tecnico };

        _userRepoMock.Setup(x => x.GetByIdAsync(tecnicoId, It.IsAny<bool>())).ReturnsAsync(tecnico);
        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, It.IsAny<bool>())).ReturnsAsync((Ticket?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.CreateAsync(ticketId, dto, tecnicoId, UserRole.Tecnico.ToString()));

        _solutionRepoMock.Verify(x => x.CreateAsync(It.IsAny<Solution>()), Times.Never);
    }

    [Fact]
    public async Task GetSolutionsByTicketAsync_ShouldReturnSolutions_WhenTicketHasSolutions()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var tecnicoId = Guid.NewGuid();
        var solutions = new List<Solution>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                TecnicoId = tecnicoId,
                Descripcion = "Solución de prueba",
                FechaCreacion = DateTime.UtcNow
            }
        };

        _solutionRepoMock.Setup(x => x.GetByTicketIdAsync(ticketId))
            .ReturnsAsync(solutions);

        // Act
        var result = await _service.GetSolutionsByTicketAsync(ticketId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Solución de prueba", result.First().Description);
        _solutionRepoMock.Verify(x => x.GetByTicketIdAsync(ticketId), Times.Once);
    }
}
