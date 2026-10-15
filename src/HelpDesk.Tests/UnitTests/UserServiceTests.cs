using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.BusinessLogic.Services;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Exceptions;
using HelpDesk.Shared.Security;
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
    private readonly Mock<IRatingRepository> _ratingRepoMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
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
        _ratingRepoMock = new Mock<IRatingRepository>();
        _tokenServiceMock = new Mock<ITokenService>();

        _service = new UserService(
            _ticketRepoMock.Object,
            _userRepoMock.Object,
            _categoryRepoMock.Object,
            _priorityRepoMock.Object,
            _statusRepoMock.Object,
            _commentRepoMock.Object,
            _statusHistoryRepoMock.Object,
            _teamRepoMock.Object,
            _ratingRepoMock.Object,
            _tokenServiceMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser_WhenValidData()
    {
        // Arrange
        var dto = new UserCreateDTO("test@test.com", "Test User", "Empleado");
        _userRepoMock.Setup(x => x.GetByEmailAsync(dto.Email, true)).ReturnsAsync((User?)null);

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
        Assert.NotNull(capturedUser?.PasswordHash);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenEmailExists()
    {
        // Arrange
        var dto = new UserCreateDTO("test@test.com", "Test User", "Empleado");
        _userRepoMock.Setup(x => x.GetByEmailAsync(dto.Email, true)).ReturnsAsync(new Empleado { Email = dto.Email });

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
    public async Task RegisterAsync_ShouldCreateEmpleado_WhenValidData()
    {
        // Arrange
        var dto = new RegisterDTO("nuevo@empresa.com", "Nuevo Empleado", "Password123!");
        _userRepoMock.Setup(x => x.GetByEmailAsync(dto.Email, true)).ReturnsAsync((User?)null);

        User? captured = null;
        _userRepoMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => captured = u)
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _service.RegisterAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("nuevo@empresa.com", result.Email);
        Assert.Equal("Empleado", result.Rol);
        Assert.NotNull(captured?.PasswordHash);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowDuplicate_WhenEmailAlreadyExists()
    {
        // Arrange
        var dto = new RegisterDTO("duplicado@empresa.com", "Usuario Existente", "Password123!");
        _userRepoMock.Setup(x => x.GetByEmailAsync(dto.Email, true)).ReturnsAsync(new Empleado { Email = dto.Email });

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateException>(() => _service.RegisterAsync(dto));
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
        _ticketRepoMock.Setup(x => x.GetByTecnicoIdAsync(userId, true))
            .ReturnsAsync(new List<Ticket> { new Ticket { Estado = new Status { EsFinal = false } } });

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeactivateAsync(userId));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenValidCredentials()
    {
        // Arrange
        var email = "test@test.com";
        var password = "Password123!";
        var passwordHash = PasswordHasher.HashPassword(password);
        var user = new Empleado
        {
            Id = Guid.NewGuid(),
            Email = email,
            NombreCompleto = "Test User",
            Activo = true,
            PasswordHash = passwordHash
        };

        _userRepoMock.Setup(x => x.GetByEmailAsync(email, true)).ReturnsAsync(user);
        _tokenServiceMock.Setup(x => x.GenerateToken(user)).Returns("valid.jwt.token");

        // Act
        var result = await _service.LoginAsync(new LoginDTO(email, password));

        // Assert
        Assert.NotNull(result.Token);
        Assert.Equal("valid.jwt.token", result.Token);
        Assert.Equal(email, result.Usuario.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        _userRepoMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), true)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedActionException>(() => _service.LoginAsync(new LoginDTO("notfound@test.com", "Password123!")));
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordIsIncorrect()
    {
        // Arrange
        var email = "test@test.com";
        var user = new Empleado
        {
            Id = Guid.NewGuid(),
            Email = email,
            Activo = true,
            PasswordHash = PasswordHasher.HashPassword("CorrectPassword123!")
        };

        _userRepoMock.Setup(x => x.GetByEmailAsync(email, true)).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedActionException>(() => _service.LoginAsync(new LoginDTO(email, "WrongPassword!")));
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUserIsInactive()
    {
        // Arrange
        var email = "inactivo@test.com";
        var user = new Empleado
        {
            Id = Guid.NewGuid(),
            Email = email,
            Activo = false,
            PasswordHash = PasswordHasher.HashPassword("Password123!")
        };

        _userRepoMock.Setup(x => x.GetByEmailAsync(email, true)).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedActionException>(() => _service.LoginAsync(new LoginDTO(email, "Password123!")));
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowValidation_WhenEmptyCredentials()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.LoginAsync(new LoginDTO("", "")));
    }
}