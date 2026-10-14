using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

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
        var result = await _service.SendMessageAsync(senderId, receiverId, text);
        return CreatedAtAction(nameof(GetConversation), new { userId = result.SenderId, contactId = result.ReceiverId }, result);
    }

    [HttpGet("conversation/{userId}/{contactId}")]
    public async Task<IActionResult> GetConversation(Guid userId, Guid contactId)
    {
        var result = await _service.GetConversationAsync(userId, contactId);
        return Ok(result);
    }
}
