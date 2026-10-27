using System.ComponentModel.DataAnnotations;
using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.BusinessLogic.Services;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Exceptions;
using Moq;
using Xunit;
using ValidationException = HelpDesk.Shared.Exceptions.ValidationException;

namespace HelpDesk.UnitTests.Services;

public class ValidationEdgeCasesTests
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
    private readonly Mock<ISolutionRepository> _solutionRepoMock;
    private readonly Mock<INotificationService> _notificationServiceMock;

    private readonly TicketService _ticketService;
    private readonly SolutionService _solutionService;
    private readonly CommentService _commentService;

    public ValidationEdgeCasesTests()
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
        _solutionRepoMock = new Mock<ISolutionRepository>();
        _notificationServiceMock = new Mock<INotificationService>();

        _ticketService = new TicketService(
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

        _solutionService = new SolutionService(
            _ticketRepoMock.Object,
            _userRepoMock.Object,
            _solutionRepoMock.Object,
            _statusHistoryRepoMock.Object,
            _statusRepoMock.Object);

        _commentService = new CommentService(
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

    // ==========================================
    // 1. CASO DE USO: CREAR TICKET
    // ==========================================

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateTicket_ShouldThrowValidation_WhenTitleIsBlank(string invalidTitle)
    {
        var dto = new TicketCreateDTO(invalidTitle, "Descripción válida de más de 10 caracteres", Guid.NewGuid(), Guid.NewGuid());
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.CreateAsync(dto, Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateTicket_ShouldThrowValidation_WhenTitleExceedsMaxLength()
    {
        var ultraLongTitle = new string('A', 201);
        var dto = new TicketCreateDTO(ultraLongTitle, "Descripción válida de más de 10 caracteres", Guid.NewGuid(), Guid.NewGuid());
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.CreateAsync(dto, Guid.NewGuid()));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateTicket_ShouldThrowValidation_WhenDescriptionIsBlank(string invalidDesc)
    {
        var dto = new TicketCreateDTO("Título válido", invalidDesc, Guid.NewGuid(), Guid.NewGuid());
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.CreateAsync(dto, Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateTicket_ShouldThrowValidation_WhenDescriptionExceedsMaxLength()
    {
        var ultraLongDesc = new string('B', 5001);
        var dto = new TicketCreateDTO("Título válido", ultraLongDesc, Guid.NewGuid(), Guid.NewGuid());
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.CreateAsync(dto, Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateTicket_ShouldThrowValidation_WhenEmptyGuidForPriorityOrCategory()
    {
        var dtoEmptyPriority = new TicketCreateDTO("Título válido", "Descripción con más de diez caracteres", Guid.Empty, Guid.NewGuid());
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.CreateAsync(dtoEmptyPriority, Guid.NewGuid()));

        var dtoEmptyCategory = new TicketCreateDTO("Título válido", "Descripción con más de diez caracteres", Guid.NewGuid(), Guid.Empty);
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.CreateAsync(dtoEmptyCategory, Guid.NewGuid()));
    }

    [Fact]
    public void CreateTicket_DataAnnotations_ShouldBeConfigured_OnConstructorParameters()
    {
        var parameters = typeof(TicketCreateDTO).GetConstructors().First().GetParameters();
        var tituloParam = parameters.First(p => p.Name == "Titulo");
        var descParam = parameters.First(p => p.Name == "Descripcion");

        Assert.True(tituloParam.GetCustomAttributes(typeof(RequiredAttribute), false).Any());
        Assert.True(tituloParam.GetCustomAttributes(typeof(MinLengthAttribute), false).Any());
        Assert.True(tituloParam.GetCustomAttributes(typeof(MaxLengthAttribute), false).Any());

        Assert.True(descParam.GetCustomAttributes(typeof(RequiredAttribute), false).Any());
        Assert.True(descParam.GetCustomAttributes(typeof(MinLengthAttribute), false).Any());
        Assert.True(descParam.GetCustomAttributes(typeof(MaxLengthAttribute), false).Any());
    }

    // ==========================================
    // 2. CASO DE USO: ELIMINAR TICKET
    // ==========================================

    [Fact]
    public async Task DeleteTicket_ShouldThrowValidation_WhenTicketIdIsEmptyGuid()
    {
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.DeleteTicketAsync(Guid.Empty, "Supervisor"));
    }

    [Fact]
    public async Task DeleteTicket_ShouldThrowUnauthorized_WhenRoleIsNotSupervisor()
    {
        var ticketId = Guid.NewGuid();
        await Assert.ThrowsAsync<UnauthorizedActionException>(() => _ticketService.DeleteTicketAsync(ticketId, "Empleado"));
        await Assert.ThrowsAsync<UnauthorizedActionException>(() => _ticketService.DeleteTicketAsync(ticketId, "Tecnico"));
    }

    [Fact]
    public async Task DeleteTicket_ShouldThrowNotFound_WhenTicketDoesNotExist()
    {
        var ticketId = Guid.NewGuid();
        _ticketRepoMock.Setup(r => r.GetByIdAsync(ticketId, false)).ReturnsAsync((Ticket?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _ticketService.DeleteTicketAsync(ticketId, "Supervisor"));
    }

    [Fact]
    public async Task DeleteTicket_ShouldThrowBusinessRule_WhenTicketIsNotOpen()
    {
        var ticketId = Guid.NewGuid();
        var statusInProgressId = Guid.NewGuid();
        var statusAbiertoId = Guid.NewGuid();

        var ticket = new Ticket
        {
            Id = ticketId,
            EstadoId = statusInProgressId,
            Estado = new Status { Id = statusInProgressId, Nombre = "En Progreso", EsFinal = false }
        };

        _ticketRepoMock.Setup(r => r.GetByIdAsync(ticketId, false)).ReturnsAsync(ticket);
        _statusRepoMock.Setup(r => r.GetInitialStatusAsync()).ReturnsAsync(new Status { Id = statusAbiertoId, Nombre = "Abierto" });

        await Assert.ThrowsAsync<BusinessRuleException>(() => _ticketService.DeleteTicketAsync(ticketId, "Supervisor"));
    }

    // ==========================================
    // 3. CASO DE USO: CAMBIAR ESTADO
    // ==========================================

    [Fact]
    public async Task ChangeStatus_ShouldThrowValidation_WhenTicketIdOrStatusIdIsEmptyGuid()
    {
        var validDto = new TicketStatusDTO(Guid.NewGuid());
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.ChangeStatusAsync(Guid.Empty, validDto, Guid.NewGuid(), "Supervisor"));

        var emptyStatusDto = new TicketStatusDTO(Guid.Empty);
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.ChangeStatusAsync(Guid.NewGuid(), emptyStatusDto, Guid.NewGuid(), "Supervisor"));
    }

    [Fact]
    public async Task ChangeStatus_ShouldThrowValidation_WhenObservationExceedsMaxLength()
    {
        var ultraLongObs = new string('X', 1001);
        var dto = new TicketStatusDTO(Guid.NewGuid(), ultraLongObs);
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.ChangeStatusAsync(Guid.NewGuid(), dto, Guid.NewGuid(), "Supervisor"));
    }

    [Fact]
    public async Task ChangeStatus_ShouldThrowBusinessRule_WhenTicketIsClosed()
    {
        var ticketId = Guid.NewGuid();
        var cerradoStatusId = Guid.NewGuid();
        var ticket = new Ticket
        {
            Id = ticketId,
            EstadoId = cerradoStatusId,
            Estado = new Status { Id = cerradoStatusId, Nombre = "Cerrado", EsFinal = true }
        };

        _ticketRepoMock.Setup(r => r.GetByIdAsync(ticketId, false)).ReturnsAsync(ticket);
        _statusRepoMock.Setup(r => r.GetByIdAsync(cerradoStatusId, true)).ReturnsAsync(ticket.Estado);

        var dto = new TicketStatusDTO(Guid.NewGuid());
        await Assert.ThrowsAsync<BusinessRuleException>(() => _ticketService.ChangeStatusAsync(ticketId, dto, Guid.NewGuid(), "Supervisor"));
    }

    // ==========================================
    // 4. CASO DE USO: ASIGNAR TICKET
    // ==========================================

    [Fact]
    public async Task AssignTechnician_ShouldThrowValidation_WhenTicketIdOrTechnicianIdIsEmptyGuid()
    {
        var validDto = new TicketAssignDTO(Guid.NewGuid());
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.AssignTechnicianAsync(Guid.Empty, validDto, Guid.NewGuid()));

        var emptyTechDto = new TicketAssignDTO(Guid.Empty);
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.AssignTechnicianAsync(Guid.NewGuid(), emptyTechDto, Guid.NewGuid()));
    }

    [Fact]
    public async Task AssignTechnician_ShouldThrowUnauthorized_WhenAssignerIsNotSupervisor()
    {
        var supervisorId = Guid.NewGuid();
        var userEmpleado = new Empleado { Id = supervisorId, Rol = UserRole.Empleado };
        _userRepoMock.Setup(u => u.GetByIdAsync(supervisorId, true)).ReturnsAsync(userEmpleado);

        var dto = new TicketAssignDTO(Guid.NewGuid());
        await Assert.ThrowsAsync<UnauthorizedActionException>(() => _ticketService.AssignTechnicianAsync(Guid.NewGuid(), dto, supervisorId));
    }

    // ==========================================
    // 5. CASO DE USO: ADJUNTAR SOLUCIÓN
    // ==========================================

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AttachSolution_ShouldThrowValidation_WhenDescriptionIsBlank(string invalidDesc)
    {
        var dto = new SolutionCreateDTO(invalidDesc);
        await Assert.ThrowsAsync<ValidationException>(() => _solutionService.CreateAsync(Guid.NewGuid(), dto, Guid.NewGuid(), "Tecnico"));
    }

    [Fact]
    public async Task AttachSolution_ShouldThrowValidation_WhenDescriptionExceedsMaxLength()
    {
        var ultraLongDesc = new string('S', 5001);
        var dto = new SolutionCreateDTO(ultraLongDesc);
        await Assert.ThrowsAsync<ValidationException>(() => _solutionService.CreateAsync(Guid.NewGuid(), dto, Guid.NewGuid(), "Tecnico"));
    }

    [Fact]
    public async Task AttachSolution_ShouldThrowValidation_WhenTicketIdOrTechnicianIdIsEmptyGuid()
    {
        var dto = new SolutionCreateDTO("Solución válida de más de diez caracteres");
        await Assert.ThrowsAsync<ValidationException>(() => _solutionService.CreateAsync(Guid.Empty, dto, Guid.NewGuid(), "Tecnico"));
        await Assert.ThrowsAsync<ValidationException>(() => _solutionService.CreateAsync(Guid.NewGuid(), dto, Guid.Empty, "Tecnico"));
    }

    [Fact]
    public void AttachSolution_DataAnnotations_ShouldBeConfigured_OnConstructorParameters()
    {
        var parameters = typeof(SolutionCreateDTO).GetConstructors().First().GetParameters();
        var descParam = parameters.First(p => p.Name == "Descripcion");

        Assert.True(descParam.GetCustomAttributes(typeof(RequiredAttribute), false).Any());
        Assert.True(descParam.GetCustomAttributes(typeof(MinLengthAttribute), false).Any());
        Assert.True(descParam.GetCustomAttributes(typeof(MaxLengthAttribute), false).Any());
    }

    // ==========================================
    // 6. CASO DE USO: CALIFICAR (RATING)
    // ==========================================

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public async Task CreateRating_ShouldThrowValidation_WhenScoreOutOfRange(int invalidScore)
    {
        var dto = new RatingCreateDTO(invalidScore, "Buen servicio");
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.CreateRatingAsync(Guid.NewGuid(), dto, Guid.NewGuid(), "Empleado"));
    }

    [Fact]
    public async Task CreateRating_ShouldThrowValidation_WhenCommentExceedsMaxLength()
    {
        var ultraLongComment = new string('C', 1001);
        var dto = new RatingCreateDTO(5, ultraLongComment);
        await Assert.ThrowsAsync<ValidationException>(() => _ticketService.CreateRatingAsync(Guid.NewGuid(), dto, Guid.NewGuid(), "Empleado"));
    }

    // ==========================================
    // 7. CASO DE USO: COMENTARIOS
    // ==========================================

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateComment_ShouldThrowValidation_WhenContentIsBlank(string invalidContent)
    {
        var dto = new CommentCreateDTO(invalidContent);
        await Assert.ThrowsAsync<ValidationException>(() => _commentService.CreateAsync(Guid.NewGuid(), dto, Guid.NewGuid(), "Empleado"));
    }

    [Fact]
    public async Task CreateComment_ShouldThrowValidation_WhenContentExceedsMaxLength()
    {
        var ultraLongComment = new string('M', 2001);
        var dto = new CommentCreateDTO(ultraLongComment);
        await Assert.ThrowsAsync<ValidationException>(() => _commentService.CreateAsync(Guid.NewGuid(), dto, Guid.NewGuid(), "Empleado"));
    }
}
