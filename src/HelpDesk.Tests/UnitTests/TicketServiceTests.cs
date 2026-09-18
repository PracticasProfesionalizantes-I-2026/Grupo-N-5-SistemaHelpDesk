using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.BusinessLogic.Services;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Exceptions;
using Moq;
using Xunit;

namespace HelpDesk.UnitTests.Services;

public class TicketServiceTests
{
    private readonly Mock<ITicketRepository> _ticketRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly Mock<IPriorityRepository> _priorityRepoMock;
    private readonly Mock<IStatusRepository> _statusRepoMock;
    private readonly Mock<ICommentRepository> _commentRepoMock;
    private readonly Mock<IStatusHistoryRepository> _statusHistoryRepoMock;
    private readonly Mock<ITeamRepository> _teamRepoMock;
    private readonly TicketService _service;

    public TicketServiceTests()
    {
        _ticketRepoMock = new Mock<ITicketRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _priorityRepoMock = new Mock<IPriorityRepository>();
        _statusRepoMock = new Mock<IStatusRepository>();
        _commentRepoMock = new Mock<ICommentRepository>();
        _statusHistoryRepoMock = new Mock<IStatusHistoryRepository>();
        _teamRepoMock = new Mock<ITeamRepository>();

        _service = new TicketService(
            _ticketRepoMock.Object,
            _userRepoMock.Object,
            _categoryRepoMock.Object,
            _priorityRepoMock.Object,
            _statusRepoMock.Object,
            _commentRepoMock.Object,
            _statusHistoryRepoMock.Object,
            _teamRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTicket_WhenValidData()
    {
        // Arrange
        var empleadoId = Guid.NewGuid();
        var categoriaId = Guid.NewGuid();
        var prioridadId = Guid.NewGuid();
        var dto = new TicketCreateDTO("Test Ticket", "Description", prioridadId, categoriaId);
        
        _userRepoMock.Setup(x => x.GetByIdAsync(empleadoId)).ReturnsAsync(new Empleado { Id = empleadoId, Activo = true });
        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoriaId)).ReturnsAsync(new Category { Id = categoriaId, Activo = true });
        _priorityRepoMock.Setup(x => x.GetByIdAsync(prioridadId)).ReturnsAsync(new Priority { Id = prioridadId, Nivel = 2, SLAHoras = 24 });
        _statusRepoMock.Setup(x => x.GetInitialStatusAsync()).ReturnsAsync(new Status { Id = Guid.NewGuid(), Nombre = "Abierto", EsFinal = false });
        
        Ticket? capturedTicket = null;
        _ticketRepoMock.Setup(x => x.CreateAsync(It.IsAny<Ticket>()))
            .Callback<Ticket>(t => capturedTicket = t)
            .ReturnsAsync((Ticket t) => t);

        _statusHistoryRepoMock.Setup(x => x.CreateAsync(It.IsAny<StatusHistory>()))
            .ReturnsAsync((StatusHistory h) => h);

        // Act
        var result = await _service.CreateAsync(dto, empleadoId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Titulo, result.Titulo);
        Assert.Equal(dto.Descripcion, result.Descripcion);
        _ticketRepoMock.Verify(x => x.CreateAsync(It.IsAny<Ticket>()), Times.Once);
        _statusHistoryRepoMock.Verify(x => x.CreateAsync(It.IsAny<StatusHistory>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowNotFound_WhenEmpleadoNotFound()
    {
        // Arrange
        var dto = new TicketCreateDTO("Test", "Desc", Guid.NewGuid(), Guid.NewGuid());
        _userRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto, Guid.NewGuid()));
    }

    [Fact]
    public async Task AssignTechnicianAsync_ShouldAssign_WhenValidSupervisorAndTechnician()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var supervisorId = Guid.NewGuid();
        var tecnicoId = Guid.NewGuid();
        var dto = new TicketAssignDTO(tecnicoId);

        _userRepoMock.Setup(x => x.GetByIdAsync(supervisorId)).ReturnsAsync(new Supervisor { Id = supervisorId });
        _userRepoMock.Setup(x => x.GetByIdAsync(tecnicoId)).ReturnsAsync(new Tecnico { Id = tecnicoId, Activo = true });

        var ticket = new Ticket
        {
            Id = ticketId,
            EstadoId = Guid.NewGuid(),
            TecnicoId = null
        };
        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, false)).ReturnsAsync(ticket);
        _statusRepoMock.Setup(x => x.GetByNameAsync("En Progreso")).ReturnsAsync(new Status { Id = Guid.NewGuid(), Nombre = "En Progreso", EsFinal = false });
        _statusHistoryRepoMock.Setup(x => x.CreateAsync(It.IsAny<StatusHistory>())).ReturnsAsync((StatusHistory h) => h);

        // Act
        var result = await _service.AssignTechnicianAsync(ticketId, dto, supervisorId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tecnicoId, ticket.TecnicoId);
        _ticketRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Ticket>()), Times.Once);
    }

    [Fact]
    public async Task AssignTechnicianAsync_ShouldThrow_WhenTicketAlreadyAssigned()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var supervisorId = Guid.NewGuid();
        var tecnicoId = Guid.NewGuid();

        _userRepoMock.Setup(x => x.GetByIdAsync(supervisorId)).ReturnsAsync(new Supervisor { Id = supervisorId });
        _userRepoMock.Setup(x => x.GetByIdAsync(tecnicoId)).ReturnsAsync(new Tecnico { Id = tecnicoId, Activo = true });

        var ticket = new Ticket
        {
            Id = ticketId,
            EstadoId = Guid.NewGuid(),
            TecnicoId = Guid.NewGuid() // Already assigned
        };
        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, false)).ReturnsAsync(ticket);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => _service.AssignTechnicianAsync(ticketId, new TicketAssignDTO(tecnicoId), supervisorId));
    }

    [Fact]
    public async Task ChangeStatusAsync_ShouldChangeStatus_WhenValidTransition()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var newStatusId = Guid.NewGuid();
        var dto = new TicketStatusDTO(newStatusId, "Testing");

        var ticket = new Ticket
        {
            Id = ticketId,
            EstadoId = Guid.NewGuid(),
            EmpleadoId = usuarioId
        };

        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, false)).ReturnsAsync(ticket);
        _statusRepoMock.Setup(x => x.GetByIdAsync(newStatusId)).ReturnsAsync(new Status { Id = newStatusId, Nombre = "En Progreso", EsFinal = false });
        _statusHistoryRepoMock.Setup(x => x.CreateAsync(It.IsAny<StatusHistory>())).ReturnsAsync((StatusHistory h) => h);

        // Act
        var result = await _service.ChangeStatusAsync(ticketId, dto, usuarioId, UserRole.Empleado.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newStatusId, ticket.EstadoId);
    }

    [Fact]
    public async Task ChangeStatusAsync_ShouldThrow_WhenTicketClosed()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var newStatusId = Guid.NewGuid();

        var ticket = new Ticket
        {
            Id = ticketId,
            Estado = new Status { EsFinal = true }, // Closed ticket
            EmpleadoId = usuarioId
        };

        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, false)).ReturnsAsync(ticket);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => _service.ChangeStatusAsync(ticketId, new TicketStatusDTO(newStatusId), usuarioId, UserRole.Empleado.ToString()));
    }

    [Fact]
    public async Task ReopenAsync_ShouldReopen_WhenSupervisorAndTicketClosed()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var supervisorId = Guid.NewGuid();

        _userRepoMock.Setup(x => x.GetByIdAsync(supervisorId)).ReturnsAsync(new Supervisor { Id = supervisorId });

        var ticket = new Ticket
        {
            Id = ticketId,
            Estado = new Status { EsFinal = true, Nombre = "Cerrado" }
        };

        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, false)).ReturnsAsync(ticket);
        _statusRepoMock.Setup(x => x.GetByNameAsync("Abierto")).ReturnsAsync(new Status { Id = Guid.NewGuid(), Nombre = "Abierto", EsFinal = false });
        _statusHistoryRepoMock.Setup(x => x.CreateAsync(It.IsAny<StatusHistory>())).ReturnsAsync((StatusHistory h) => h);

        // Act
        var result = await _service.ReopenAsync(ticketId, supervisorId);

        // Assert
        Assert.NotNull(result);
        Assert.False(ticket.Estado.EsFinal);
    }

    [Fact]
    public async Task ReopenAsync_ShouldThrow_WhenNotSupervisor()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var tecnicoId = Guid.NewGuid();

        _userRepoMock.Setup(x => x.GetByIdAsync(tecnicoId)).ReturnsAsync(new Tecnico { Id = tecnicoId });

        var ticket = new Ticket
        {
            Id = ticketId,
            Estado = new Status { EsFinal = true }
        };

        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, false)).ReturnsAsync(ticket);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedActionException>(() => _service.ReopenAsync(ticketId, tecnicoId));
    }
}