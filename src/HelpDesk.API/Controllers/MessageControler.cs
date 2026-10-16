using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _service;

    public MessagesController(IMessageService service)
    {
        _service = service;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(Guid senderId, Guid receiverId, string text)
    {
        try
        {
            var result = await _service.SendMessageAsync(senderId, receiverId, text);
            return CreatedAtAction(nameof(GetConversation), new { userId = result.SenderId, contactId = result.ReceiverId }, result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("conversation/{userId}/{contactId}")]
    public async Task<IActionResult> GetConversation(Guid userId, Guid contactId)
    {
        try
        {
            var result = await _service.GetConversationAsync(userId, contactId);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
