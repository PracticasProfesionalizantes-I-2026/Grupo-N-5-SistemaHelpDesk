using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;

namespace HelpDesk.BusinessLogic.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<NotificationResponseDTO>> GetMyNotificationsAsync(Guid usuarioId)
    {
        var notifications = await _notificationRepository.GetByUsuarioIdAsync(usuarioId);

        return notifications.Select(n =>
            new NotificationResponseDTO(
                n.Id,
                n.Titulo,
                n.Mensaje,
                n.Leida,
                n.FechaCreacion
            )
        );
    }

    public async Task EnviarNotificacionAsync(Guid ticketId, Guid usuarioId, string titulo, string mensaje, string canal = "Panel")
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            TicketId = ticketId,
            Titulo = titulo,
            Mensaje = mensaje,
            FechaCreacion = DateTime.UtcNow,
            Leida = false,
            Entregada = true,
            Canal = canal
        };

        await _notificationRepository.RegistrarAsync(notification);
    }

    public async Task MarcarComoLeidaAsync(Guid notificationId)
    {
        await _notificationRepository.MarcarComoLeidaAsync(notificationId);
    }

    public async Task ActualizarEstadoAsync(Guid notificationId, bool entregada)
    {
        await _notificationRepository.ActualizarEstadoAsync(notificationId, entregada);
    }
}