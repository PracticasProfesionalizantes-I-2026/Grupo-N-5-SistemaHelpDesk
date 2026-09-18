using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDTO>> Create([FromBody] UserCreateDTO dto)
    {
        try
        {
            var result = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDTO<UserListDTO>>> GetAll(
        [FromQuery] string? rol = null,
        [FromQuery] bool? activo = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var filter = new UserFilterDTO(rol, activo, search, page, pageSize);
            var result = await _userService.GetFilteredAsync(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDTO>> GetById(Guid id)
    {
        try
        {
            var result = await _userService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { error = "Usuario no encontrado" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponseDTO>> Update(Guid id, [FromBody] UserUpdateDTO dto)
    {
        try
        {
            var result = await _userService.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<ActionResult> Deactivate(Guid id)
    {
        try
        {
            await _userService.DeactivateAsync(id);
            return NoContent();
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
            _ => StatusCode(500, new { error = "Error interno del servidor" })
        };
    }
}