using System.IdentityModel.Tokens.Jwt;
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
    // =========================================================================
    // ROLES PARA PROBAR EN BRUNO (DESCOMENTAR EL QUE QUIERAS PROBAR POR DEFECTO)
    // O enviar headers opcionales en Bruno:
    // X-User-Role: Supervisor | Tecnico | Empleado
    // X-User-Id: <guid>
    // =========================================================================

    // 1. SUPERVISOR (Admin Sistema)
    //private static readonly Guid DefaultUserId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
    //private static readonly string DefaultUserRole = "Supervisor";

    // 2. TÉCNICO (Carlos López)
    private static readonly Guid DefaultUserId = Guid.Parse("cccccccc-dddd-eeee-ffff-aaaaaaaaaaaa");
    private static readonly string DefaultUserRole = "Tecnico";

    // 3. EMPLEADO (Juan Pérez)
    //private static readonly Guid DefaultUserId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
    //private static readonly string DefaultUserRole = "Empleado";

    protected Guid GetCurrentUserId()
    {
        // 1. Si viene por Claims JWT (Bearer Token)
        var claim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User?.FindFirst("sub")?.Value
            ?? User?.FindFirst("id")?.Value;

        if (!string.IsNullOrEmpty(claim) && Guid.TryParse(claim, out var parsedClaimId))
        {
            return parsedClaimId;
        }

        // 2. Si viene por header opcional en Bruno / Postman / Tests (X-User-Id)
        if (Request?.Headers.TryGetValue("X-User-Id", out var headerId) == true &&
            Guid.TryParse(headerId, out var parsedHeaderId))
        {
            return parsedHeaderId;
        }

        // 3. Fallback hardcodeado
        return DefaultUserId;
    }

    protected string GetCurrentUserRole()
    {
        // 1. Si viene por Claims JWT (Bearer Token)
        var claim = User?.FindFirst(ClaimTypes.Role)?.Value
            ?? User?.FindFirst("role")?.Value;

        if (!string.IsNullOrEmpty(claim))
        {
            return claim;
        }

        // 2. Si viene por header opcional en Bruno / Postman / Tests (X-User-Role)
        if (Request?.Headers.TryGetValue("X-User-Role", out var headerRole) == true &&
            !string.IsNullOrWhiteSpace(headerRole))
        {
            return headerRole.ToString();
        }

        // 3. Fallback hardcodeado
        return DefaultUserRole;
    }
}
