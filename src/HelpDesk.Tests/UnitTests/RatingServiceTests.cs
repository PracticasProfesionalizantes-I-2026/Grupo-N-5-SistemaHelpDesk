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

/// <summary>
/// Tests para CU-07: Calificar atención
/// El método real es TicketService.CreateRatingAsync(ticketId, dto, usuarioId, usuarioRol)
/// Precondición: ticket en estado "Resuelto" o "Cerrado"
/// </summary>
public class RatingServiceTests
{
    private readonly Mock<ITicketRepository> _ticketRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly Mock<IPriorityRepository> _priorityRepoMock;
    private readonly Mock<IStatusRepository> _statusRepoMock;
    private readonly Mock<ICommentRepository> _commentRepoMock;
    private readonly Mock<IStatusHistoryRepository> _statusHistoryRepoMock;
    private readonly Mock<ITeamRepository> _teamRepoMock;
    private readonly Mock<IRatingRepository> _ratingRepoMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly TicketService _service;

    public RatingServiceTests()
    {
        _ticketRepoMock = new Mock<ITicketRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _priorityRepoMock = new Mock<IPriorityRepository>();
        _statusRepoMock = new Mock<IStatusRepository>();
        _commentRepoMock = new Mock<ICommentRepository>();
        _statusHistoryRepoMock = new Mock<IStatusHistoryRepository>();
        _teamRepoMock = new Mock<ITeamRepository>();
        _ratingRepoMock = new Mock<IRatingRepository>();
        _notificationServiceMock = new Mock<INotificationService>();

        _service = new TicketService(
            _ticketRepoMock.Object,
            _userRepoMock.Object,
            _categoryRepoMock.Object,
            _priorityRepoMock.Object,
            _statusRepoMock.Object,
            _commentRepoMock.Object,
            _statusHistoryRepoMock.Object,
            _teamRepoMock.Object,
            _ratingRepoMock.Object,
            _notificationServiceMock.Object);
    }

    [Fact]
    public async Task CreateRatingAsync_ShouldCreateRating_WhenTicketIsResuelto()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var empleadoId = Guid.NewGuid();
        var estadoId = Guid.NewGuid();

        var ticket = new Ticket
        {
            Id = ticketId,
            EmpleadoId = empleadoId,
            EstadoId = estadoId
        };

        var estado = new Status { Id = estadoId, Nombre = "Resuelto", EsFinal = true };
        var dto = new RatingCreateDTO(5, "Excelente atención");

        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, true)).ReturnsAsync(ticket);
        _statusRepoMock.Setup(x => x.GetByIdAsync(estadoId, true)).ReturnsAsync(estado);
        _ratingRepoMock.Setup(x => x.GetByTicketIdAsync(ticketId, true)).ReturnsAsync((Rating?)null);

        Rating? capturedRating = null;
        _ratingRepoMock.Setup(x => x.CreateAsync(It.IsAny<Rating>()))
            .Callback<Rating>(r => capturedRating = r)
            .ReturnsAsync((Rating r) =>
            {
                r.Id = Guid.NewGuid();
                r.Usuario = new Empleado { Id = empleadoId, NombreCompleto = "Test User" };
                return r;
            });

        // Act
        var result = await _service.CreateRatingAsync(ticketId, dto, empleadoId, "Empleado");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Puntuacion);
        _ratingRepoMock.Verify(x => x.CreateAsync(It.IsAny<Rating>()), Times.Once);
    }

    [Fact]
    public async Task CreateRatingAsync_ShouldCreateRating_WhenTicketIsCerrado()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var empleadoId = Guid.NewGuid();
        var estadoId = Guid.NewGuid();

        var ticket = new Ticket
        {
            Id = ticketId,
            EmpleadoId = empleadoId,
            EstadoId = estadoId
        };

        var estado = new Status { Id = estadoId, Nombre = "Cerrado", EsFinal = true };
        var dto = new RatingCreateDTO(4, "Buen servicio");

        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, true)).ReturnsAsync(ticket);
        _statusRepoMock.Setup(x => x.GetByIdAsync(estadoId, true)).ReturnsAsync(estado);
        _ratingRepoMock.Setup(x => x.GetByTicketIdAsync(ticketId, true)).ReturnsAsync((Rating?)null);
        _ratingRepoMock.Setup(x => x.CreateAsync(It.IsAny<Rating>()))
            .ReturnsAsync((Rating r) =>
            {
                r.Id = Guid.NewGuid();
                r.Usuario = new Empleado { Id = empleadoId, NombreCompleto = "Test" };
                return r;
            });

        // Act
        var result = await _service.CreateRatingAsync(ticketId, dto, empleadoId, "Empleado");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Puntuacion);
        _ratingRepoMock.Verify(x => x.CreateAsync(It.IsAny<Rating>()), Times.Once);
    }

    [Fact]
    public async Task CreateRatingAsync_ShouldThrow_WhenTicketNotResolvedOrClosed()
    {
        // Arrange — ticket en estado "En Progreso" (no final)
        var ticketId = Guid.NewGuid();
        var empleadoId = Guid.NewGuid();
        var estadoId = Guid.NewGuid();

        var ticket = new Ticket
        {
            Id = ticketId,
            EmpleadoId = empleadoId,
            EstadoId = estadoId
        };

        var estado = new Status { Id = estadoId, Nombre = "En Progreso", EsFinal = false };
        var dto = new RatingCreateDTO(5, "Test");

        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, true)).ReturnsAsync(ticket);
        _statusRepoMock.Setup(x => x.GetByIdAsync(estadoId, true)).ReturnsAsync(estado);
        _ratingRepoMock.Setup(x => x.GetByTicketIdAsync(ticketId, true)).ReturnsAsync((Rating?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.CreateRatingAsync(ticketId, dto, empleadoId, "Empleado"));

        _ratingRepoMock.Verify(x => x.CreateAsync(It.IsAny<Rating>()), Times.Never);
    }

    [Fact]
    public async Task CreateRatingAsync_ShouldThrow_WhenTicketNotFound()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var dto = new RatingCreateDTO(5, "Test");

        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, true)).ReturnsAsync((Ticket?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.CreateRatingAsync(ticketId, dto, Guid.NewGuid(), "Empleado"));
    }
}
