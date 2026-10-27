using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

namespace HelpDesk.API.Controllers;

// [Authorize]
[ApiController]
[Route("api/tickets/{ticketId}/[controller]")]
public class CommentsController : BaseApiController
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
            if (ticketId == Guid.Empty)
                return BadRequest(new { error = "El identificador del ticket no es válido", message = "El identificador del ticket no es válido" });

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
            if (ticketId == Guid.Empty)
                return BadRequest(new { error = "El identificador del ticket no es válido", message = "El identificador del ticket no es válido" });

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



    private ActionResult HandleException(Exception ex)
    {
        return ex switch
        {
            NotFoundException => NotFound(new { error = ex.Message, message = ex.Message }),
            ValidationException => BadRequest(new { error = ex.Message, message = ex.Message }),
            BusinessRuleException => Conflict(new { error = ex.Message, message = ex.Message }),
            UnauthorizedActionException => StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message, message = ex.Message }),
            UnauthorizedAccessException => StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message, message = ex.Message }),
            _ => StatusCode(500, new { error = "Error interno del servidor", message = "Error interno del servidor" })
        };
    }
}