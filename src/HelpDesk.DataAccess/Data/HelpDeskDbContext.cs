using HelpDesk.DataAccess.Entities;
using HelpDesk.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.DataAccess.Data;

/// <summary>
/// DbContext de EF Core que configura las entidades y relaciones del sistema HelpDesk.
/// </summary>
public class HelpDeskDbContext : DbContext
{
    /// <summary>Inicializa una nueva instancia del <see cref="HelpDeskDbContext"/>.</summary>
    /// <param name="options">Opciones de configuración del contexto.</param>
    public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options) : base(options) { }

    /// <summary>Conjunto de usuarios del sistema.</summary>
    public DbSet<User> Usuarios => Set<User>();

    /// <summary>Conjunto de tickets de soporte.</summary>
    public DbSet<Ticket> Tickets => Set<Ticket>();

    /// <summary>Conjunto de categorías de soporte.</summary>
    public DbSet<Category> Categorias => Set<Category>();

    /// <summary>Conjunto de prioridades de atención.</summary>
    public DbSet<Priority> Prioridades => Set<Priority>();

    /// <summary>Conjunto de estados de los tickets.</summary>
    public DbSet<Status> Estados => Set<Status>();

    /// <summary>Conjunto de comentarios de los tickets.</summary>
    public DbSet<Comment> Comentarios => Set<Comment>();

    /// <summary>Conjunto del historial de cambios de estado de los tickets.</summary>
    public DbSet<StatusHistory> HistorialEstados => Set<StatusHistory>();

    /// <summary>Conjunto de equipos de trabajo.</summary>
    public DbSet<Team> Equipos => Set<Team>();

    /// <summary>Configura las entidades, relaciones y datos semilla del modelo.</summary>
    /// <param name="modelBuilder">Constructor del modelo.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de herencia User (TPH - Table Per Hierarchy)
        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("TipoUsuario")
            .HasValue<Empleado>("Empleado")
            .HasValue<Tecnico>("Tecnico")
            .HasValue<Supervisor>("Supervisor");

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.NombreCompleto)
            .HasMaxLength(150)
            .IsRequired();

        // Ticket
        modelBuilder.Entity<Ticket>()
            .Property(t => t.Titulo)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Entity<Ticket>()
            .Property(t => t.Descripcion)
            .HasMaxLength(5000)
            .IsRequired();

        modelBuilder.Entity<Ticket>()
            .Property(t => t.RowVersion)
            .IsRowVersion();

        // Relaciones Ticket
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Prioridad)
            .WithMany(p => p.Tickets)
            .HasForeignKey(t => t.PrioridadId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Estado)
            .WithMany(e => e.Tickets)
            .HasForeignKey(t => t.EstadoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Categoria)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Empleado)
            .WithMany(u => u.TicketsCreados)
            .HasForeignKey(t => t.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Tecnico)
            .WithMany(u => u.TicketsAsignados)
            .HasForeignKey(t => t.TecnicoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Team)
            .WithMany(tm => tm.Tickets)
            .HasForeignKey(t => t.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        // Category
        modelBuilder.Entity<Category>()
            .Property(c => c.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Category>()
            .Property(c => c.Descripcion)
            .HasMaxLength(500);

        modelBuilder.Entity<Category>()
            .Property(c => c.RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Nombre)
            .IsUnique();

        // Priority
        modelBuilder.Entity<Priority>()
            .Property(p => p.Nombre)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Priority>()
            .Property(p => p.Color)
            .HasMaxLength(7)
            .IsRequired();

        modelBuilder.Entity<Priority>()
            .Property(p => p.RowVersion)
            .IsRowVersion();

        // Status
        modelBuilder.Entity<Status>()
            .Property(s => s.Nombre)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Status>()
            .Property(s => s.Descripcion)
            .HasMaxLength(200);

        modelBuilder.Entity<Status>()
            .Property(s => s.RowVersion)
            .IsRowVersion();

        // Comment
        modelBuilder.Entity<Comment>()
            .Property(c => c.Contenido)
            .HasMaxLength(3000)
            .IsRequired();

        modelBuilder.Entity<Comment>()
            .Property(c => c.RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Ticket)
            .WithMany(t => t.Comentarios)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Comentarios)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // StatusHistory
        modelBuilder.Entity<StatusHistory>()
            .Property(h => h.Observacion)
            .HasMaxLength(500);

        modelBuilder.Entity<StatusHistory>()
            .Property(h => h.RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<StatusHistory>()
            .HasOne(h => h.Ticket)
            .WithMany(t => t.HistorialEstados)
            .HasForeignKey(h => h.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StatusHistory>()
            .HasOne(h => h.EstadoAnterior)
            .WithMany(s => s.HistorialEstadoAnterior)
            .HasForeignKey(h => h.EstadoAnteriorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StatusHistory>()
            .HasOne(h => h.EstadoNuevo)
            .WithMany(s => s.HistorialEstadoNuevo)
            .HasForeignKey(h => h.EstadoNuevoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StatusHistory>()
            .HasOne(h => h.Usuario)
            .WithMany(u => u.HistorialEstados)
            .HasForeignKey(h => h.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Team
        modelBuilder.Entity<Team>()
            .Property(t => t.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Team>()
            .Property(t => t.Descripcion)
            .HasMaxLength(500);

        modelBuilder.Entity<Team>()
            .Property(t => t.RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<Team>()
            .HasIndex(t => t.Nombre)
            .IsUnique();

        modelBuilder.Entity<Team>()
            .HasOne(t => t.Categoria)
            .WithMany()
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Many-to-Many Team <-> Tecnico (User)
        modelBuilder.Entity<Team>()
            .HasMany(t => t.Tecnicos)
            .WithMany(u => u.Equipos)
            .UsingEntity<Dictionary<string, object>>(
                "TeamTecnico",
                j => j.HasOne<User>().WithMany().HasForeignKey("TecnicoId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Team>().WithMany().HasForeignKey("TeamId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("TeamId", "TecnicoId");
                    j.ToTable("TeamTecnicos");
                });

        // Seed Data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Prioridades
        var prioridades = new List<Priority>
        {
            new Priority { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Nombre = "Baja", Nivel = 1, Color = "#16A34A", SLAHoras = 72 },
            new Priority { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Nombre = "Media", Nivel = 2, Color = "#CA8A04", SLAHoras = 24 },
            new Priority { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Nombre = "Alta", Nivel = 3, Color = "#EA580C", SLAHoras = 8 },
            new Priority { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Nombre = "Crítica", Nivel = 4, Color = "#DC2626", SLAHoras = 4 }
        };
        modelBuilder.Entity<Priority>().HasData(prioridades);

        // Estados
        var estados = new List<Status>
        {
            new Status { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Nombre = "Abierto", Descripcion = "Ticket recién creado", EsFinal = false, Orden = 1 },
            new Status { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Nombre = "En Progreso", Descripcion = "Ticket siendo trabajado", EsFinal = false, Orden = 2 },
            new Status { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Nombre = "Resuelto", Descripcion = "Ticket resuelto, pendiente de cierre", EsFinal = false, Orden = 3 },
            new Status { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), Nombre = "Cerrado", Descripcion = "Ticket cerrado definitivamente", EsFinal = true, Orden = 4 },
            new Status { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Nombre = "Reabierto", Descripcion = "Ticket reabierto después de cerrado", EsFinal = false, Orden = 5 }
        };
        modelBuilder.Entity<Status>().HasData(estados);

        // Categorías
        var categorias = new List<Category>
        {
            new Category { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Nombre = "Hardware", Descripcion = "Problemas de hardware", Activo = true },
            new Category { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Nombre = "Software", Descripcion = "Problemas de software", Activo = true },
            new Category { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Nombre = "Red", Descripcion = "Problemas de red y conectividad", Activo = true },
            new Category { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Nombre = "Accesos", Descripcion = "Gestión de accesos y permisos", Activo = true },
            new Category { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Nombre = "Otro", Descripcion = "Otras consultas", Activo = true }
        };
        modelBuilder.Entity<Category>().HasData(categorias);

        // Usuarios
        var usuarios = new List<User>
        {
            new Supervisor { Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), Email = "admin@helpdesk.com", NombreCompleto = "Admin Sistema", FechaCreacion = DateTime.UtcNow, Activo = true },
            new Empleado { Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), Email = "juan.perez@empresa.com", NombreCompleto = "Juan Pérez", FechaCreacion = DateTime.UtcNow, Activo = true },
            new Empleado { Id = Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"), Email = "maria.garcia@empresa.com", NombreCompleto = "María García", FechaCreacion = DateTime.UtcNow, Activo = true },
            new Tecnico { Id = Guid.Parse("cccccccc-dddd-eeee-ffff-aaaaaaaaaaaa"), Email = "carlos.lopez@empresa.com", NombreCompleto = "Carlos López", FechaCreacion = DateTime.UtcNow, Activo = true },
            new Tecnico { Id = Guid.Parse("dddddddd-eeee-ffff-aaaa-bbbbbbbbbbbb"), Email = "ana.martinez@empresa.com", NombreCompleto = "Ana Martínez", FechaCreacion = DateTime.UtcNow, Activo = true }
        };
        modelBuilder.Entity<User>().HasData(usuarios);
    }
}