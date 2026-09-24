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

public class CategoryServiceTests
{
    private readonly Mock<ITicketRepository> _ticketRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly Mock<IPriorityRepository> _priorityRepoMock;
    private readonly Mock<IStatusRepository> _statusRepoMock;
    private readonly Mock<ICommentRepository> _commentRepoMock;
    private readonly Mock<IStatusHistoryRepository> _statusHistoryRepoMock;
    private readonly Mock<ITeamRepository> _teamRepoMock;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _ticketRepoMock = new Mock<ITicketRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _priorityRepoMock = new Mock<IPriorityRepository>();
        _statusRepoMock = new Mock<IStatusRepository>();
        _commentRepoMock = new Mock<ICommentRepository>();
        _statusHistoryRepoMock = new Mock<IStatusHistoryRepository>();
        _teamRepoMock = new Mock<ITeamRepository>();

        _service = new CategoryService(
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
    public async Task CreateAsync_ShouldCreateCategory_WhenValidData()
    {
        // Arrange
        var dto = new CategoryCreateDTO("Hardware", "Hardware issues");
        _categoryRepoMock.Setup(x => x.GetByNameAsync(dto.Nombre)).ReturnsAsync((Category?)null);
        
        Category? capturedCategory = null;
        _categoryRepoMock.Setup(x => x.CreateAsync(It.IsAny<Category>()))
            .Callback<Category>(c => capturedCategory = c)
            .ReturnsAsync((Category c) => c);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Nombre, result.Nombre);
        Assert.Equal(dto.Descripcion, result.Descripcion);
        Assert.True(result.Activo);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenNameExists()
    {
        // Arrange
        var dto = new CategoryCreateDTO("Hardware", "Hardware issues");
        _categoryRepoMock.Setup(x => x.GetByNameAsync(dto.Nombre)).ReturnsAsync(new Category { Nombre = dto.Nombre });

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenNoTickets()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Nombre = "Test" };
        
        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId, false)).ReturnsAsync(category);
        _ticketRepoMock.Setup(x => x.GetFilteredAsync(null, null, categoryId, null, null, null, null, 1, 20, true)).ReturnsAsync(new List<Ticket>());

        // Act
        await _service.DeleteAsync(categoryId);

        // Assert
        _categoryRepoMock.Verify(x => x.DeleteAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenHasTickets()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Nombre = "Test" };
        
        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId, false)).ReturnsAsync(category);
        _ticketRepoMock.Setup(x => x.GetFilteredAsync(null, null, categoryId, null, null, null, null, 1, 20, true))
            .ReturnsAsync(new List<Ticket> { new Ticket() });

        // Act & Assert
        await Assert.ThrowsAsync<DependencyException>(() => _service.DeleteAsync(categoryId));
    }
}

public class TeamServiceTests
{
    private readonly Mock<ITicketRepository> _ticketRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly Mock<IPriorityRepository> _priorityRepoMock;
    private readonly Mock<IStatusRepository> _statusRepoMock;
    private readonly Mock<ICommentRepository> _commentRepoMock;
    private readonly Mock<IStatusHistoryRepository> _statusHistoryRepoMock;
    private readonly Mock<ITeamRepository> _teamRepoMock;
    private readonly TeamService _service;

    public TeamServiceTests()
    {
        _ticketRepoMock = new Mock<ITicketRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _priorityRepoMock = new Mock<IPriorityRepository>();
        _statusRepoMock = new Mock<IStatusRepository>();
        _commentRepoMock = new Mock<ICommentRepository>();
        _statusHistoryRepoMock = new Mock<IStatusHistoryRepository>();
        _teamRepoMock = new Mock<ITeamRepository>();

        _service = new TeamService(
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
    public async Task CreateAsync_ShouldCreateTeam_WhenValidData()
    {
        // Arrange
        var categoriaId = Guid.NewGuid();
        var tecnicoId = Guid.NewGuid();
        var dto = new TeamCreateDTO("Team A", "Description", categoriaId, new[] { tecnicoId });
        
        _teamRepoMock.Setup(x => x.GetByNameAsync(dto.Nombre)).ReturnsAsync((Team?)null);
        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoriaId)).ReturnsAsync(new Category { Id = categoriaId, Activo = true });
        _userRepoMock.Setup(x => x.GetByIdAsync(tecnicoId)).ReturnsAsync(new Tecnico { Id = tecnicoId, Activo = true, Rol = UserRole.Tecnico });
        
        Team? capturedTeam = null;
        _teamRepoMock.Setup(x => x.CreateAsync(It.IsAny<Team>()))
            .Callback<Team>(t => capturedTeam = t)
            .ReturnsAsync((Team t) => t);
        _teamRepoMock.Setup(x => x.AddTecnicoAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
        _teamRepoMock.Setup(x => x.GetWithTecnicosAsync(It.IsAny<Guid>())).ReturnsAsync((Team?)null);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Nombre, result.Nombre);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenNoTechnicians()
    {
        // Arrange
        var dto = new TeamCreateDTO("Team A", "Desc", Guid.NewGuid(), Array.Empty<Guid>());

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task AddTecnicoAsync_ShouldAdd_WhenValidTechnician()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var tecnicoId = Guid.NewGuid();
        var dto = new TeamAddTecnicoDTO(tecnicoId);

        _teamRepoMock.Setup(x => x.GetByIdAsync(teamId, false)).ReturnsAsync(new Team { Id = teamId });
        _userRepoMock.Setup(x => x.GetByIdAsync(tecnicoId)).ReturnsAsync(new Tecnico { Id = tecnicoId, Activo = true, Rol = UserRole.Tecnico });
        _teamRepoMock.Setup(x => x.HasTecnicoAsync(teamId, tecnicoId)).ReturnsAsync(false);
        _teamRepoMock.Setup(x => x.GetWithTecnicosAsync(teamId)).ReturnsAsync(new Team { Id = teamId, Tecnicos = new List<User> { new Tecnico { Id = tecnicoId } } });

        // Act
        var result = await _service.AddTecnicoAsync(teamId, dto);

        // Assert
        Assert.NotNull(result);
        _teamRepoMock.Verify(x => x.AddTecnicoAsync(teamId, tecnicoId), Times.Once);
    }
}