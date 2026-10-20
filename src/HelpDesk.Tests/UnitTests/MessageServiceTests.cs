using HelpDesk.BusinessLogic.Services;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Exceptions;
using Moq;
using Xunit;

namespace HelpDesk.UnitTests.Services;

public class MessageServiceTests
{
    private readonly Mock<IMessageRepository> _messageRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly MessageService _service;

    public MessageServiceTests()
    {
        _messageRepoMock = new Mock<IMessageRepository>();
        _userRepoMock = new Mock<IUserRepository>();

        _service = new MessageService(
            _messageRepoMock.Object,
            _userRepoMock.Object);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldSendMessage_WhenValidSenderAndReceiver()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();
        var text = "Hola, ¿cómo estás?";

        var sender = new Empleado
        {
            Id = senderId,
            Email = "sender@test.com",
            NombreCompleto = "Sender User",
            Rol = UserRole.Empleado,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        var receiver = new Tecnico
        {
            Id = receiverId,
            Email = "receiver@test.com",
            NombreCompleto = "Receiver Tech",
            Rol = UserRole.Tecnico,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        _userRepoMock.Setup(x => x.GetByIdAsync(senderId, true)).ReturnsAsync(sender);
        _userRepoMock.Setup(x => x.GetByIdAsync(receiverId, true)).ReturnsAsync(receiver);

        Message? capturedMessage = null;
        _messageRepoMock.Setup(x => x.AddAsync(It.IsAny<Message>()))
            .Callback<Message>(m => capturedMessage = m)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SendMessageAsync(senderId, receiverId, text);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(text, result.Text);
        Assert.Equal(senderId, result.SenderId);
        Assert.Equal(receiverId, result.ReceiverId);
        Assert.NotNull(capturedMessage);
        Assert.Equal(text, capturedMessage.Text);
        _messageRepoMock.Verify(x => x.AddAsync(It.IsAny<Message>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldThrowNotFound_WhenSenderDoesNotExist()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();

        _userRepoMock.Setup(x => x.GetByIdAsync(senderId, true)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.SendMessageAsync(senderId, receiverId, "Texto"));

        _messageRepoMock.Verify(x => x.AddAsync(It.IsAny<Message>()), Times.Never);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldThrowNotFound_WhenReceiverDoesNotExist()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();

        var sender = new Empleado { Id = senderId, Activo = true };
        _userRepoMock.Setup(x => x.GetByIdAsync(senderId, true)).ReturnsAsync(sender);
        _userRepoMock.Setup(x => x.GetByIdAsync(receiverId, true)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.SendMessageAsync(senderId, receiverId, "Texto"));

        _messageRepoMock.Verify(x => x.AddAsync(It.IsAny<Message>()), Times.Never);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldThrowValidation_WhenTextIsEmpty()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.SendMessageAsync(senderId, receiverId, "   "));

        _messageRepoMock.Verify(x => x.AddAsync(It.IsAny<Message>()), Times.Never);
    }

    [Fact]
    public async Task GetConversationAsync_ShouldReturnMessages_WhenValidUsers()
    {
        // Arrange
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();

        var user1 = new Empleado { Id = user1Id, Email = "u1@test.com", NombreCompleto = "User 1", Rol = UserRole.Empleado, Activo = true };
        var user2 = new Tecnico { Id = user2Id, Email = "u2@test.com", NombreCompleto = "User 2", Rol = UserRole.Tecnico, Activo = true };

        var messages = new List<Message>
        {
            new()
            {
                Id = Guid.NewGuid(),
                SenderId = user1Id,
                ReceiverId = user2Id,
                Text = "Hola",
                SentAt = DateTime.UtcNow,
                Sender = user1,
                Receiver = user2
            }
        };

        _messageRepoMock.Setup(x => x.GetConversationAsync(user1Id, user2Id))
            .ReturnsAsync(messages);

        // Act
        var result = await _service.GetConversationAsync(user1Id, user2Id);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Hola", result.First().Text);
        _messageRepoMock.Verify(x => x.GetConversationAsync(user1Id, user2Id), Times.Once);
    }
}
