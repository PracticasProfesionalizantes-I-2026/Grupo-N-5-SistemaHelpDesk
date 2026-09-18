using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrioritiesController : ControllerBase
{
    private readonly IPriorityService _priorityService;

    public PrioritiesController(IPriorityService priorityService)
    {
        _priorityService = priorityService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PriorityResponseDTO>>> GetAll()
    {
        var result = await _priorityService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PriorityResponseDTO>> GetById(Guid id)
    {
        var result = await _priorityService.GetByIdAsync(id);
        if (result == null)
            return NotFound(new { error = "Prioridad no encontrada" });
        return Ok(result);
    }
}