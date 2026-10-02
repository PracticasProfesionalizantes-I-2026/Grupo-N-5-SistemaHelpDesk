using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginDTO dto)
    {
        try
        {
            var result = await _userService.LoginAsync(dto);
            return Ok(result);
        }
        catch (UnauthorizedActionException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDTO>> Register([FromBody] RegisterDTO dto)
    {
        try
        {
            var result = await _userService.RegisterAsync(dto);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (UnauthorizedActionException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
}