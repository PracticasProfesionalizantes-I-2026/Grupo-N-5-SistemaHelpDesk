using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStatsDTO>> GetDashboardStats()
    {
        try
        {
            var result = await _reportService.GetDashboardStatsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("tickets-by-status")]
    public async Task<ActionResult<IEnumerable<TicketsByStatusDTO>>> GetTicketsByStatus()
    {
        try
        {
            var result = await _reportService.GetTicketsByStatusAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("tickets-by-priority")]
    public async Task<ActionResult<IEnumerable<TicketsByPriorityDTO>>> GetTicketsByPriority()
    {
        try
        {
            var result = await _reportService.GetTicketsByPriorityAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("tickets-by-technician")]
    public async Task<ActionResult<IEnumerable<TicketsByTechnicianDTO>>> GetTicketsByTechnician()
    {
        try
        {
            var result = await _reportService.GetTicketsByTechnicianAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("sla-compliance")]
    public async Task<ActionResult<IEnumerable<SLAComplianceDTO>>> GetSLACompliance()
    {
        try
        {
            var result = await _reportService.GetSLAComplianceAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("technician-workload")]
    public async Task<ActionResult<IEnumerable<TechnicianWorkloadDTO>>> GetTechnicianWorkload()
    {
        try
        {
            var result = await _reportService.GetTechnicianWorkloadAsync();
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
            UnauthorizedActionException => StatusCode(403, new { error = ex.Message }),
            _ => StatusCode(500, new { error = "Error interno del servidor" })
        };
    }
}