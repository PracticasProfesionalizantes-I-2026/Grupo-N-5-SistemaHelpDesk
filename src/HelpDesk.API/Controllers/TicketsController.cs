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

    [HttpGet("{id}/history")]
    public async Task<ActionResult<IEnumerable<StatusHistoryResponseDTO>>> GetHistory(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userRol = GetCurrentUserRole();
            var result = await _ticketService.GetHistoryAsync(id, userId, userRol);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(Guid id)
    {
        try
        {
            string userRole = GetCurrentUserRole();
            await _ticketService.DeleteTicketAsync(id, userRole);
            return NoContent(); // 204: Eliminado correctamente
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message }); // 409: Estado inválido
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message }); // 404: No existe
        }
    }
    [HttpPut("{id}/resolve")]
    public async Task<IActionResult> ResolveTicket(Guid id, [FromQuery] bool cerrar = false)
    {
        try
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            var usuarioId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value!);

            await _ticketService.ResolveAsync(id, userRole!, usuarioId, cerrar);
            return NoContent(); // 204
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error interno al resolver el ticket", detail = ex.Message });
        }
    }



    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var supervisorId = GetCurrentUserId();
            await _ticketService.ResolveAsync(id, supervisorId, true);
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }
    [HttpPost("{id}/rating")]
    public async Task<ActionResult<RatingResponseDTO>> CreateRating(Guid id, [FromBody] RatingCreateDTO dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userRol = GetCurrentUserRole();
            var result = await _ticketService.CreateRatingAsync(id, dto, userId, userRol);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("{id}/rating")]
    public async Task<ActionResult<RatingResponseDTO>> GetRating(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userRol = GetCurrentUserRole();
            var result = await _ticketService.GetRatingByTicketIdAsync(id, userId, userRol);

            if (result == null)
                return NotFound(new { error = "Este ticket aún no fue calificado" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost("escalate-overdue")]
    public async Task<ActionResult<object>> EscalateOverdue()
    {
        try
        {
            var count = await _ticketService.EscalateOverdueTicketsAsync();
            return Ok(new { escalados = count, mensaje = $"Se escalaron {count} ticket(s) por SLA" });
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