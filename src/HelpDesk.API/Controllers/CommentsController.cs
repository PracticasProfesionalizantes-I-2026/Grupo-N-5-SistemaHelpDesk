using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/tickets/{ticketId}/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<ActionResult<CommentResponseDTO>> Create(Guid ticketId, [FromBody] CommentCreateDTO dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userRol = GetCurrentUserRole();
            var result = await _commentService.CreateAsync(ticketId, dto, userId, userRol);
            return CreatedAtAction(nameof(GetByTicketId), new { ticketId }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentResponseDTO>>> GetByTicketId(Guid ticketId, [FromQuery] bool soloPublicos = false)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userRol = GetCurrentUserRole();
            var result = await _commentService.GetByTicketIdAsync(ticketId, userId, userRol, soloPublicos);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    private Guid GetCurrentUserId() => Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
    private string GetCurrentUserRole() => "Empleado";

    private ActionResult HandleException(Exception ex)
    {
        return ex switch
        {
            NotFoundException => NotFound(new { error = ex.Message }),
            ValidationException => BadRequest(new { error = ex.Message }),
            BusinessRuleException => Conflict(new { error = ex.Message }),
            UnauthorizedActionException => Forbid(),
            _ => StatusCode(500, new { error = "Error interno del servidor" })
        };
    }
}