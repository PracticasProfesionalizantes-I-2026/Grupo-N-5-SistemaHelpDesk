using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<NotificationResponseDTO>>> GetByUsuario(Guid usuarioId)
    {
        var result = await _notificationService.GetMyNotificationsAsync(usuarioId);
        return Ok(result);
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(Guid ticketId, Guid usuarioId, string titulo, string mensaje)
    {
        await _notificationService.EnviarNotificacionAsync(ticketId, usuarioId, titulo, mensaje);
        return Ok(new { message = "Notificación enviada correctamente." });
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await _notificationService.MarcarComoLeidaAsync(id);
        return Ok(new { message = "Notificación marcada como leída." });
    }
}
