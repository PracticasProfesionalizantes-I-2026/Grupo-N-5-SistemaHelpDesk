using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpPost]
    public async Task<ActionResult<TeamResponseDTO>> Create([FromBody] TeamCreateDTO dto)
    {
        try
        {
            var result = await _teamService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDTO<TeamListDTO>>> GetAll(
        [FromQuery] Guid? categoriaId = null,
        [FromQuery] bool? activo = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var filter = new TeamFilterDTO(categoriaId, activo, search, page, pageSize);
            var result = await _teamService.GetFilteredAsync(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeamResponseDTO>> GetById(Guid id)
    {
        try
        {
            var result = await _teamService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { error = "Equipo no encontrado" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TeamResponseDTO>> Update(Guid id, [FromBody] TeamUpdateDTO dto)
    {
        try
        {
            var result = await _teamService.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _teamService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost("{id}/tecnicos")]
    public async Task<ActionResult<TeamResponseDTO>> AddTecnico(Guid id, [FromBody] TeamAddTecnicoDTO dto)
    {
        try
        {
            var result = await _teamService.AddTecnicoAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpDelete("{id}/tecnicos/{tecnicoId}")]
    public async Task<ActionResult<TeamResponseDTO>> RemoveTecnico(Guid id, Guid tecnicoId)
    {
        try
        {
            var dto = new TeamRemoveTecnicoDTO(tecnicoId);
            var result = await _teamService.RemoveTecnicoAsync(id, dto);
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
            NotFoundException => NotFound(new { error = ex.Message }),
            ValidationException => BadRequest(new { error = ex.Message }),
            BusinessRuleException => Conflict(new { error = ex.Message }),
            DuplicateException => Conflict(new { error = ex.Message }),
            DependencyException => Conflict(new { error = ex.Message }),
            _ => StatusCode(500, new { error = "Error interno del servidor" })
        };
    }
}