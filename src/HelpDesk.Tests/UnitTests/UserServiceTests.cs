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

public class UserServiceTests
{
    private readonly Mock<ITicketRepository> _ticketRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly Mock<IPriorityRepository> _priorityRepoMock;
    private readonly Mock<IStatusRepository> _statusRepoMock;
    private readonly Mock<ICommentRepository> _commentRepoMock;
    private readonly Mock<IStatusHistoryRepository> _statusHistoryRepoMock;
    private readonly Mock<ITeamRepository> _teamRepoMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _ticketRepoMock = new Mock<ITicketRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _priorityRepoMock = new Mock<IPriorityRepository>();
        _statusRepoMock = new Mock<IStatusRepository>();
        _commentRepoMock = new Mock<ICommentRepository>();
        _statusHistoryRepoMock = new Mock<IStatusHistoryRepository>();
        _teamRepoMock = new Mock<ITeamRepository>();

        _service = new UserService(
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
    public async Task CreateAsync_ShouldCreateUser_WhenValidData()
    {
        // Arrange
        var dto = new UserCreateDTO("test@test.com", "Test User", "Empleado");
        _userRepoMock.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
        
        User? capturedUser = null;
        _userRepoMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Email, result.Email);
        Assert.Equal(dto.NombreCompleto, result.NombreCompleto);
        Assert.Equal("Empleado", result.Rol);
        Assert.True(result.Activo);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenEmailExists()
    {
        // Arrange
        var dto = new UserCreateDTO("test@test.com", "Test User", "Empleado");
        _userRepoMock.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync(new Empleado { Email = dto.Email });

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenInvalidRole()
    {
        // Arrange
        var dto = new UserCreateDTO("test@test.com", "Test User", "InvalidRole");

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenValidData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Empleado { Id = userId, Email = "old@test.com", NombreCompleto = "Old Name", Activo = true };
        var dto = new UserUpdateDTO("New Name", "Tecnico", true);

        _userRepoMock.Setup(x => x.GetByIdAsync(userId, false)).ReturnsAsync(user);

        // Act
        var result = await _service.UpdateAsync(userId, dto);

        // Assert
        Assert.Equal("New Name", result.NombreCompleto);
        Assert.Equal("Tecnico", result.Rol);
    }

    [Fact]
    public async Task DeactivateAsync_ShouldThrow_WhenUserHasOpenTickets()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Tecnico { Id = userId, Activo = true };
        
        _userRepoMock.Setup(x => x.GetByIdAsync(userId, false)).ReturnsAsync(user);
        _ticketRepoMock.Setup(x => x.GetByTecnicoIdAsync(userId))
            .ReturnsAsync(new List<Ticket> { new Ticket { Estado = new Status { EsFinal = false } } });

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeactivateAsync(userId));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenValidCredentials()
    {
        // Arrange
        var email = "test@test.com";
        var user = new Empleado { Id = Guid.NewGuid(), Email = email, NombreCompleto = "Test", Activo = true };
        
        _userRepoMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);

        // Act
        var result = await _service.LoginAsync(new LoginDTO(email));

        // Assert
        Assert.NotNull(result.Token);
        Assert.Equal(email, result.Usuario.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        _userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedActionException>(() => _service.LoginAsync(new LoginDTO("notfound@test.com")));
    }
}