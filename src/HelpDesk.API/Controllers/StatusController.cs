using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    private readonly IStatusService _statusService;

    public StatusController(IStatusService statusService)
    {
        _statusService = statusService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StatusResponseDTO>>> GetAll()
    {
        var result = await _statusService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StatusResponseDTO>> GetById(Guid id)
    {
        var result = await _statusService.GetByIdAsync(id);
        if (result == null)
            return NotFound(new { error = "Estado no encontrado" });
        return Ok(result);
    }

    [HttpGet("initial")]
    public async Task<ActionResult<StatusResponseDTO>> GetInitial()
    {
        var result = await _statusService.GetInitialStatusAsync();
        if (result == null)
            return NotFound(new { error = "Estado inicial no configurado" });
        return Ok(result);
    }

    [HttpGet("closed")]
    public async Task<ActionResult<StatusResponseDTO>> GetClosed()
    {
        var result = await _statusService.GetClosedStatusAsync();
        if (result == null)
            return NotFound(new { error = "Estado cerrado no configurado" });
        return Ok(result);
    }
}