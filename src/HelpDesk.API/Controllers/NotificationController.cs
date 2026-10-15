using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : BaseApiController
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("mias")]
    public async Task<ActionResult<IEnumerable<NotificationResponseDTO>>> GetMias()
    {
        var currentUserId = GetCurrentUserId();
        var result = await _notificationService.GetMyNotificationsAsync(currentUserId);
        return Ok(result);
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<NotificationResponseDTO>>> GetByUsuario(Guid usuarioId)
    {
        var currentUserId = GetCurrentUserId();
        var currentRole = GetCurrentUserRole();

        if (currentRole != "Supervisor" && currentUserId != usuarioId)
        {
            return Forbid();
        }

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
