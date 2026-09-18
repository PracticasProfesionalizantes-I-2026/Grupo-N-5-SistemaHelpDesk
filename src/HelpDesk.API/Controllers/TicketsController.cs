using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponseDTO>> Create([FromBody] TicketCreateDTO dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _ticketService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDTO<TicketListDTO>>> GetAll(
        [FromQuery] Guid? estadoId = null,
        [FromQuery] Guid? prioridadId = null,
        [FromQuery] Guid? categoriaId = null,
        [FromQuery] Guid? tecnicoId = null,
        [FromQuery] Guid? empleadoId = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var filter = new TicketFilterDTO(estadoId, prioridadId, categoriaId, tecnicoId, empleadoId, fechaDesde, fechaHasta, page, pageSize);
            var userId = GetCurrentUserId();
            var userRol = GetCurrentUserRole();
            var result = await _ticketService.GetFilteredAsync(filter, userId, userRol);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketResponseDTO>> GetById(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userRol = GetCurrentUserRole();
            var result = await _ticketService.GetByIdAsync(id, userId, userRol);
            
            if (result == null)
                return NotFound(new { error = "Ticket no encontrado" });
                
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TicketResponseDTO>> Update(Guid id, [FromBody] TicketUpdateDTO dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userRol = GetCurrentUserRole();
            var result = await _ticketService.UpdateAsync(id, dto, userId, userRol);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPatch("{id}/assign")]
    public async Task<ActionResult<TicketResponseDTO>> AssignTechnician(Guid id, [FromBody] TicketAssignDTO dto)
    {
        try
        {
            var supervisorId = GetCurrentUserId();
            var result = await _ticketService.AssignTechnicianAsync(id, dto, supervisorId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<TicketResponseDTO>> ChangeStatus(Guid id, [FromBody] TicketStatusDTO dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userRol = GetCurrentUserRole();
            var result = await _ticketService.ChangeStatusAsync(id, dto, userId, userRol);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPatch("{id}/reopen")]
    public async Task<ActionResult<TicketResponseDTO>> Reopen(Guid id)
    {
        try
        {
            var supervisorId = GetCurrentUserId();
            var result = await _ticketService.ReopenAsync(id, supervisorId);
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
            var supervisorId = GetCurrentUserId();
            await _ticketService.DeleteAsync(id, supervisorId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    private Guid GetCurrentUserId()
    {
        // En producción, esto vendría del token JWT
        // Para testing, usamos un ID fijo del seed data
        return Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"); // Juan Pérez
    }

    private string GetCurrentUserRole()
    {
        // En producción, esto vendría del token JWT
        return "Empleado";
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
            UnauthorizedActionException => Forbid(),
            _ => StatusCode(500, new { error = "Error interno del servidor" })
        };
    }
}