using HelpDesk.BusinessLogic.Services;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Exceptions;
using Moq;
using Xunit;

namespace HelpDesk.UnitTests.Services;

public class CommentServiceTests
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
    private readonly CommentService _service;

    public CommentServiceTests()
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

        _service = new CommentService(
            _ticketRepoMock.Object,
            _userRepoMock.Object,
            _categoryRepoMock.Object,
            _priorityRepoMock.Object,
            _statusRepoMock.Object,
            _commentRepoMock.Object,
            _statusHistoryRepoMock.Object,
            _teamRepoMock.Object,
            _ratingRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateComment_WhenValidData()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var empleadoId = Guid.NewGuid();
        var dto = new CommentCreateDTO("Comentario de prueba", false);

        var empleado = new Empleado
        {
            Id = empleadoId,
            Email = "empleado@test.com",
            NombreCompleto = "Empleado Test",
            Rol = UserRole.Empleado,
            Activo = true
        };

        var ticket = new Ticket
        {
            Id = ticketId,
            EmpleadoId = empleadoId,
            Estado = new Status { Id = Guid.NewGuid(), Nombre = "Abierto", EsFinal = false }
        };

        _userRepoMock.Setup(x => x.GetByIdAsync(empleadoId, It.IsAny<bool>())).ReturnsAsync(empleado);
        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, It.IsAny<bool>())).ReturnsAsync(ticket);

        Comment? capturedComment = null;
        _commentRepoMock.Setup(x => x.CreateAsync(It.IsAny<Comment>()))
            .Callback<Comment>(c => capturedComment = c)
            .ReturnsAsync((Comment c) =>
            {
                c.Id = Guid.NewGuid();
                return c;
            });

        // Act
        var result = await _service.CreateAsync(ticketId, dto, empleadoId, UserRole.Empleado.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Contenido, result.Contenido);
        Assert.False(result.EsInterno);
        Assert.NotNull(capturedComment);
        Assert.Equal(ticketId, capturedComment.TicketId);
        Assert.Equal(empleadoId, capturedComment.UsuarioId);
        _commentRepoMock.Verify(x => x.CreateAsync(It.IsAny<Comment>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenTicketClosed()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var empleadoId = Guid.NewGuid();
        var dto = new CommentCreateDTO("Comentario en cerrado", false);

        var empleado = new Empleado { Id = empleadoId, Activo = true, Rol = UserRole.Empleado };
        var ticket = new Ticket
        {
            Id = ticketId,
            EmpleadoId = empleadoId,
            Estado = new Status { Id = Guid.NewGuid(), Nombre = "Cerrado", EsFinal = true }
        };

        _userRepoMock.Setup(x => x.GetByIdAsync(empleadoId, It.IsAny<bool>())).ReturnsAsync(empleado);
        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, It.IsAny<bool>())).ReturnsAsync(ticket);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.CreateAsync(ticketId, dto, empleadoId, UserRole.Empleado.ToString()));

        _commentRepoMock.Verify(x => x.CreateAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowNotFound_WhenTicketDoesNotExist()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var empleadoId = Guid.NewGuid();
        var dto = new CommentCreateDTO("Comentario", false);

        _userRepoMock.Setup(x => x.GetByIdAsync(empleadoId, It.IsAny<bool>()))
            .ReturnsAsync(new Empleado { Id = empleadoId, Activo = true });
        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, It.IsAny<bool>()))
            .ReturnsAsync((Ticket?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.CreateAsync(ticketId, dto, empleadoId, UserRole.Empleado.ToString()));

        _commentRepoMock.Verify(x => x.CreateAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldForceEsInternoFalse_WhenEmpleadoAttemptsInternalComment()
    {
        // Arrange - Empleado intenta enviar un comentario como interno
        var ticketId = Guid.NewGuid();
        var empleadoId = Guid.NewGuid();
        var dto = new CommentCreateDTO("Comentario confidencial intentado por empleado", true);

        var empleado = new Empleado { Id = empleadoId, Activo = true, Rol = UserRole.Empleado, Email = "emp@test.com", NombreCompleto = "Emp Test" };
        var ticket = new Ticket
        {
            Id = ticketId,
            EmpleadoId = empleadoId,
            Estado = new Status { EsFinal = false }
        };

        _userRepoMock.Setup(x => x.GetByIdAsync(empleadoId, It.IsAny<bool>())).ReturnsAsync(empleado);
        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, It.IsAny<bool>())).ReturnsAsync(ticket);

        Comment? capturedComment = null;
        _commentRepoMock.Setup(x => x.CreateAsync(It.IsAny<Comment>()))
            .Callback<Comment>(c => capturedComment = c)
            .ReturnsAsync((Comment c) => { c.Id = Guid.NewGuid(); return c; });

        // Act
        var result = await _service.CreateAsync(ticketId, dto, empleadoId, UserRole.Empleado.ToString());

        // Assert: El comentario debe ser forzado a público (EsInterno = false)
        Assert.False(result.EsInterno);
        Assert.NotNull(capturedComment);
        Assert.False(capturedComment.EsInterno);
    }

    [Fact]
    public async Task CreateAsync_ShouldAllowInternalComment_WhenTecnicoOrSupervisor()
    {
        // Arrange - Técnico asignado crea comentario interno
        var ticketId = Guid.NewGuid();
        var tecnicoId = Guid.NewGuid();
        var dto = new CommentCreateDTO("Nota técnica interna", true);

        var tecnico = new Tecnico { Id = tecnicoId, Activo = true, Rol = UserRole.Tecnico, Email = "tec@test.com", NombreCompleto = "Tec Test" };
        var ticket = new Ticket
        {
            Id = ticketId,
            TecnicoId = tecnicoId,
            Estado = new Status { EsFinal = false }
        };

        _userRepoMock.Setup(x => x.GetByIdAsync(tecnicoId, It.IsAny<bool>())).ReturnsAsync(tecnico);
        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, It.IsAny<bool>())).ReturnsAsync(ticket);

        Comment? capturedComment = null;
        _commentRepoMock.Setup(x => x.CreateAsync(It.IsAny<Comment>()))
            .Callback<Comment>(c => capturedComment = c)
            .ReturnsAsync((Comment c) => { c.Id = Guid.NewGuid(); return c; });

        // Act
        var result = await _service.CreateAsync(ticketId, dto, tecnicoId, UserRole.Tecnico.ToString());

        // Assert
        Assert.True(result.EsInterno);
        Assert.NotNull(capturedComment);
        Assert.True(capturedComment.EsInterno);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenUserCannotAccessTicket()
    {
        // Arrange - Empleado intenta comentar en el ticket de otro empleado
        var ticketId = Guid.NewGuid();
        var otroUsuarioId = Guid.NewGuid();
        var ticketOwnerId = Guid.NewGuid();
        var dto = new CommentCreateDTO("Comentario no autorizado", false);

        var intruso = new Empleado { Id = otroUsuarioId, Activo = true, Rol = UserRole.Empleado };
        var ticket = new Ticket
        {
            Id = ticketId,
            EmpleadoId = ticketOwnerId,
            TecnicoId = Guid.NewGuid(),
            Estado = new Status { EsFinal = false }
        };

        _userRepoMock.Setup(x => x.GetByIdAsync(otroUsuarioId, It.IsAny<bool>())).ReturnsAsync(intruso);
        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, It.IsAny<bool>())).ReturnsAsync(ticket);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedActionException>(
            () => _service.CreateAsync(ticketId, dto, otroUsuarioId, UserRole.Empleado.ToString()));

        _commentRepoMock.Verify(x => x.CreateAsync(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task GetByTicketIdAsync_ShouldReturnComments_WhenAuthorized()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var supervisorId = Guid.NewGuid();
        var ticket = new Ticket
        {
            Id = ticketId,
            EmpleadoId = Guid.NewGuid(),
            Estado = new Status { EsFinal = false }
        };

        var commentUser = new Empleado { Id = Guid.NewGuid(), NombreCompleto = "User", Email = "u@test.com", Rol = UserRole.Empleado };
        var comments = new List<Comment>
        {
            new() { Id = Guid.NewGuid(), TicketId = ticketId, UsuarioId = commentUser.Id, Contenido = "Msg 1", EsInterno = false, FechaCreacion = DateTime.UtcNow }
        };

        _ticketRepoMock.Setup(x => x.GetByIdAsync(ticketId, It.IsAny<bool>())).ReturnsAsync(ticket);
        _commentRepoMock.Setup(x => x.GetByTicketIdAsync(ticketId)).ReturnsAsync(comments);
        _userRepoMock.Setup(x => x.GetByIdAsync(commentUser.Id, It.IsAny<bool>())).ReturnsAsync(commentUser);

        // Act
        var result = await _service.GetByTicketIdAsync(ticketId, supervisorId, UserRole.Supervisor.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Msg 1", result.First().Contenido);
    }
}
