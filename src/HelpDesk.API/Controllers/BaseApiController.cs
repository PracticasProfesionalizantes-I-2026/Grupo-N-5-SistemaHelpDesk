using System.Security.Claims;
using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers;

/// <summary>
/// Controlador base para endpoints de la API con utilidades de identidad extraídas de los claims del JWT.
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Obtiene el identificador único (Guid) del usuario autenticado a partir de los claims del token.
    /// </summary>
    /// <exception cref="UnauthorizedActionException">Si el usuario no está autenticado o el claim 'sub'/'nameidentifier' es inválido.</exception>
    protected Guid GetCurrentUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
        {
            throw new UnauthorizedActionException("Usuario no autenticado o sesión inválida.");
        }

        return userId;
    }

    /// <summary>
    /// Obtiene el rol del usuario autenticado a partir de los claims del token.
    /// </summary>
    /// <exception cref="UnauthorizedActionException">Si el claim de rol no está presente en el token.</exception>
    protected string GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value
                        ?? User.FindFirst("role")?.Value;

        if (string.IsNullOrEmpty(roleClaim))
        {
            throw new UnauthorizedActionException("Rol de usuario no identificado.");
        }

        return roleClaim;
    }
}
