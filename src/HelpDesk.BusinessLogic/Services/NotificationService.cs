using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;

namespace HelpDesk.BusinessLogic.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(
        INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<NotificationResponseDTO>> GetMyNotificationsAsync(
        Guid usuarioId)
    {
        var notifications =
            await _notificationRepository.GetByUsuarioIdAsync(usuarioId);

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
}