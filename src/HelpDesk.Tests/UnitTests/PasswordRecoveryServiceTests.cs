using HelpDesk.BusinessLogic.Services;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using HelpDesk.Shared.Security;
using Moq;
using Xunit;

namespace HelpDesk.Tests.UnitTests;

public class PasswordRecoveryServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordResetTokenRepository> _tokenRepoMock;
    private readonly PasswordRecoveryService _service;

    public PasswordRecoveryServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _tokenRepoMock = new Mock<IPasswordResetTokenRepository>();
        _service = new PasswordRecoveryService(_userRepoMock.Object, _tokenRepoMock.Object);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldReturnCode_WhenUserExistsAndIsActive()
    {
        // Arrange
        var user = new Empleado
        {
            Id = Guid.NewGuid(),
            Email = "empleado@test.com",
            Activo = true
        };

        _userRepoMock.Setup(x => x.GetByEmailAsync("empleado@test.com", true))
            .ReturnsAsync(user);

        // Act
        var result = await _service.ForgotPasswordAsync(new ForgotPasswordDTO("empleado@test.com"));

        // Assert
        Assert.True(result.Exitoso);
        Assert.NotNull(result.Codigo);
        Assert.Equal(6, result.Codigo.Length);
        _tokenRepoMock.Verify(x => x.InvalidateUserTokensAsync(user.Id), Times.Once);
        _tokenRepoMock.Verify(x => x.CreateAsync(It.IsAny<PasswordResetToken>()), Times.Once);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldNotReturnCode_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepoMock.Setup(x => x.GetByEmailAsync("notfound@test.com", true))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.ForgotPasswordAsync(new ForgotPasswordDTO("notfound@test.com"));

        // Assert
        Assert.True(result.Exitoso);
        Assert.Null(result.Codigo);
        _tokenRepoMock.Verify(x => x.CreateAsync(It.IsAny<PasswordResetToken>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_ShouldThrowValidation_WhenEmailIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.ForgotPasswordAsync(new ForgotPasswordDTO("")));
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldResetPassword_WhenDataIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Empleado
        {
            Id = userId,
            Email = "user@test.com",
            Activo = true,
            PasswordHash = "OldHash"
        };

        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Codigo = "123456",
            ExpiraEn = DateTime.UtcNow.AddMinutes(10),
            Usado = false
        };

        _userRepoMock.Setup(x => x.GetByEmailAsync("user@test.com", false))
            .ReturnsAsync(user);
        _tokenRepoMock.Setup(x => x.GetValidTokenAsync(userId, "123456"))
            .ReturnsAsync(resetToken);

        // Act
        var result = await _service.ResetPasswordAsync(new ResetPasswordDTO("user@test.com", "123456", "NewPassword123!"));

        // Assert
        Assert.True(result.Exitoso);
        Assert.True(resetToken.Usado);
        _tokenRepoMock.Verify(x => x.UpdateAsync(resetToken), Times.Once);
        _userRepoMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldThrowBusinessRule_WhenTokenIsInvalidOrExpired()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Empleado
        {
            Id = userId,
            Email = "user@test.com",
            Activo = true
        };

        _userRepoMock.Setup(x => x.GetByEmailAsync("user@test.com", false))
            .ReturnsAsync(user);
        _tokenRepoMock.Setup(x => x.GetValidTokenAsync(userId, "999999"))
            .ReturnsAsync((PasswordResetToken?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _service.ResetPasswordAsync(new ResetPasswordDTO("user@test.com", "999999", "NewPassword123!")));
    }
}
