using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

namespace HelpDesk.API.Controllers;

[Authorize]
[ApiController]
[Route("api/tickets/{ticketId}/solutions")]
public class SolutionsController : BaseApiController
{
    private readonly ISolutionService _solutionService;

    public SolutionsController(
        ISolutionService solutionService)
    {
        _solutionService = solutionService;
    }

    [HttpGet("form")]
    public async Task<ActionResult<SolutionFormDTO>> GetForm(
        Guid ticketId)
    {
        try
        {
            var tecnicoId = GetCurrentUserId();

            var result =
                await _solutionService.GetFormAsync(
                    ticketId,
                    tecnicoId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost]
    [RequestSizeLimit(11 * 1024 * 1024)]
    public async Task<ActionResult<SolutionResponseDTO>> Create(
        Guid ticketId,
        [FromForm] string descripcion,
        [FromForm] IFormFile? archivo = null)
    {
        try
        {
            var tecnicoId = GetCurrentUserId();
            var tecnicoRol = GetCurrentUserRole();

            byte[]? contenido = null;

            if (archivo != null)
            {
                using var memoryStream =
                    new MemoryStream();

                await archivo.CopyToAsync(memoryStream);

                contenido =
                    memoryStream.ToArray();
            }

            var dto = new SolutionCreateDTO(
                descripcion,
                archivo?.FileName,
                archivo?.ContentType,
                archivo?.Length,
                contenido);

            var result =
                await _solutionService.CreateAsync(
                    ticketId,
                    dto,
                    tecnicoId,
                    tecnicoRol);

            return CreatedAtAction(
                nameof(GetForm),
                new { ticketId },
                result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }



    private ActionResult HandleException(
        Exception ex)
    {
        return ex switch
        {
            NotFoundException =>
                NotFound(new { error = ex.Message }),

            ValidationException =>
                BadRequest(new { error = ex.Message }),

            BusinessRuleException =>
                Conflict(new { error = ex.Message }),

            UnauthorizedActionException =>
                Forbid(),

            _ =>
                StatusCode(
                    500,
                    new
                    {
                        error =
                            "Error interno del servidor"
                    })
        };
    }
    [HttpGet]
public async Task<ActionResult<IEnumerable<SolutionResponseDTO>>> GetByTicket(Guid ticketId)
{
    var result = await _solutionService.GetSolutionsByTicketAsync(ticketId);
    return Ok(result);
}

    [HttpPost("attach")]
    public async Task<IActionResult> Attach(Guid ticketId, Guid tecnicoId, string description, IFormFile? file)
    {
        byte[]? content = null;
        long? size = null;
        string? contentType = null;
        string? fileName = null;
        string? filePath = null;

        if (file != null)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            content = ms.ToArray();
            size = file.Length;
            contentType = file.ContentType;
            fileName = file.FileName;
            filePath = $"uploads/{file.FileName}";
        }

        await _solutionService.AttachSolutionAsync(ticketId, tecnicoId, description, fileName, filePath, contentType, size, content);

        return Ok(new { message = "Solution attached successfully." });
    }
}