using HelpDesk.DataAccess.Data;
using HelpDesk.DataAccess.Entities;
using HelpDesk.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HelpDesk.DataAccess.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HelpDeskDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<HelpDeskDbContext>>();

        try
        {
            logger.LogInformation("Iniciando migración de base de datos...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Migración completada.");

            logger.LogInformation("Verificando datos de prueba...");
            await SeedDataAsync(context);
            logger.LogInformation("Datos de prueba verificados.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error durante la inicialización de la base de datos");
            throw;
        }
    }

    private static async Task SeedDataAsync(HelpDeskDbContext context)
    {
        // Verificar si ya hay datos
        if (await context.Usuarios.AnyAsync())
        {
            return; // Ya hay datos, no hacer nada
        }

        // Prioridades
        var prioridades = new List<Priority>
        {
            new Priority { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Nombre = "Baja", Nivel = 1, Color = "#16A34A", SLAHoras = 72 },
            new Priority { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Nombre = "Media", Nivel = 2, Color = "#CA8A04", SLAHoras = 24 },
            new Priority { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Nombre = "Alta", Nivel = 3, Color = "#EA580C", SLAHoras = 8 },
            new Priority { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Nombre = "Crítica", Nivel = 4, Color = "#DC2626", SLAHoras = 4 }
        };
        await context.Prioridades.AddRangeAsync(prioridades);

        // Estados
        var estados = new List<Status>
        {
            new Status { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Nombre = "Abierto", Descripcion = "Ticket recién creado", EsFinal = false, Orden = 1 },
            new Status { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Nombre = "En Progreso", Descripcion = "Ticket siendo trabajado", EsFinal = false, Orden = 2 },
            new Status { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Nombre = "Resuelto", Descripcion = "Ticket resuelto, pendiente de cierre", EsFinal = false, Orden = 3 },
            new Status { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), Nombre = "Cerrado", Descripcion = "Ticket cerrado definitivamente", EsFinal = true, Orden = 4 },
            new Status { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Nombre = "Reabierto", Descripcion = "Ticket reabierto después de cerrado", EsFinal = false, Orden = 5 }
        };
        await context.Estados.AddRangeAsync(estados);

        // Categorías
        var categorias = new List<Category>
        {
            new Category { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Nombre = "Hardware", Descripcion = "Problemas de hardware", Activo = true },
            new Category { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Nombre = "Software", Descripcion = "Problemas de software", Activo = true },
            new Category { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Nombre = "Red", Descripcion = "Problemas de red y conectividad", Activo = true },
            new Category { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Nombre = "Accesos", Descripcion = "Gestión de accesos y permisos", Activo = true },
            new Category { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Nombre = "Otro", Descripcion = "Otras consultas", Activo = true }
        };
        await context.Categorias.AddRangeAsync(categorias);

        // Usuarios
        var usuarios = new List<User>
        {
            new Supervisor { Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), Email = "admin@helpdesk.com", NombreCompleto = "Admin Sistema", FechaCreacion = DateTime.UtcNow, Activo = true },
            new Empleado { Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), Email = "juan.perez@empresa.com", NombreCompleto = "Juan Pérez", FechaCreacion = DateTime.UtcNow, Activo = true },
            new Empleado { Id = Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"), Email = "maria.garcia@empresa.com", NombreCompleto = "María García", FechaCreacion = DateTime.UtcNow, Activo = true },
            new Tecnico { Id = Guid.Parse("cccccccc-dddd-eeee-ffff-aaaaaaaaaaaa"), Email = "carlos.lopez@empresa.com", NombreCompleto = "Carlos López", FechaCreacion = DateTime.UtcNow, Activo = true },
            new Tecnico { Id = Guid.Parse("dddddddd-eeee-ffff-aaaa-bbbbbbbbbbbb"), Email = "ana.martinez@empresa.com", NombreCompleto = "Ana Martínez", FechaCreacion = DateTime.UtcNow, Activo = true }
        };
        await context.Usuarios.AddRangeAsync(usuarios);

        await context.SaveChangesAsync();
    }
}