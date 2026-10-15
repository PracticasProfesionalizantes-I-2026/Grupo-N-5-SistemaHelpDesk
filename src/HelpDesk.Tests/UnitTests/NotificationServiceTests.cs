using HelpDesk.BusinessLogic.Services;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using Moq;
using Xunit;

namespace HelpDesk.UnitTests.Services;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepoMock;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        _notificationRepoMock = new Mock<INotificationRepository>();
        _service = new NotificationService(_notificationRepoMock.Object);
    }

    [Fact]
    public async Task EnviarNotificacionAsync_ShouldRegisterNotification_WhenValidData()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var titulo = "Nuevo Ticket";
        var mensaje = "Se ha asignado un ticket";

        Notification? captured = null;
        _notificationRepoMock.Setup(x => x.RegistrarAsync(It.IsAny<Notification>()))
            .Callback<Notification>(n => captured = n)
            .Returns(Task.CompletedTask);

        // Act
        await _service.EnviarNotificacionAsync(ticketId, usuarioId, titulo, mensaje);

        // Assert
        _notificationRepoMock.Verify(x => x.RegistrarAsync(It.IsAny<Notification>()), Times.Once);
        Assert.NotNull(captured);
        Assert.Equal(ticketId, captured.TicketId);
        Assert.Equal(usuarioId, captured.UsuarioId);
        Assert.Equal(titulo, captured.Titulo);
        Assert.Equal(mensaje, captured.Mensaje);
        Assert.False(captured.Leida);
        Assert.True(captured.Entregada);
    }

    [Fact]
    public async Task GetMyNotificationsAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var notifications = new List<Notification>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                Titulo = "Notif 1",
                Mensaje = "Msg 1",
                Leida = false,
                FechaCreacion = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                Titulo = "Notif 2",
                Mensaje = "Msg 2",
                Leida = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        _notificationRepoMock.Setup(x => x.GetByUsuarioIdAsync(usuarioId, true))
            .ReturnsAsync(notifications);

        // Act
        var result = await _service.GetMyNotificationsAsync(usuarioId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, n => n.Titulo == "Notif 1" && !n.Leida);
        Assert.Contains(result, n => n.Titulo == "Notif 2" && n.Leida);
    }

    [Fact]
    public async Task MarcarComoLeidaAsync_ShouldCallRepository()
    {
        // Arrange
        var notifId = Guid.NewGuid();
        _notificationRepoMock.Setup(x => x.MarcarComoLeidaAsync(notifId)).Returns(Task.CompletedTask);

        // Act
        await _service.MarcarComoLeidaAsync(notifId);

        // Assert
        _notificationRepoMock.Verify(x => x.MarcarComoLeidaAsync(notifId), Times.Once);
    }
}
