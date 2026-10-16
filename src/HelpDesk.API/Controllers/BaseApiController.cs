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
    // ROLES PARA PROBAR EN BRUNO (DESCOMENTAR EL QUE QUIERAS PROBAR)
    // =========================================================================

    // 1. SUPERVISOR (Admin Sistema)
    protected Guid GetCurrentUserId() => Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
    protected string GetCurrentUserRole() => "Supervisor";

    // 2. TÉCNICO (Carlos López)
    // protected Guid GetCurrentUserId() => Guid.Parse("cccccccc-dddd-eeee-ffff-aaaaaaaaaaaa");
    // protected string GetCurrentUserRole() => "Tecnico";

    // 3. EMPLEADO (Juan Pérez)
    // protected Guid GetCurrentUserId() => Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
    // protected string GetCurrentUserRole() => "Empleado";
}
