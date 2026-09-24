using HelpDesk.Shared.Enums;

namespace HelpDesk.DataAccess.Entities;

/// <summary>
/// Usuario del sistema HelpDesk. Clase base de la herencia TPH con rol discriminado.
/// </summary>
public abstract class User
{
    /// <summary>Identificador único del usuario.</summary>
    public Guid Id { get; set; }

    /// <summary>Correo electrónico del usuario (único en el sistema).</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Nombre completo del usuario.</summary>
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>Rol del usuario: Empleado, Técnico o Supervisor.</summary>
    public UserRole Rol { get; set; }

    /// <summary>Fecha y hora de creación del usuario.</summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>Indica si el usuario se encuentra activo.</summary>
    public bool Activo { get; set; } = true;

    /// <summary>Tickets creados por el usuario en rol de empleado.</summary>
    public virtual ICollection<Ticket> TicketsCreados { get; set; } = new List<Ticket>();

    /// <summary>Tickets asignados al usuario en rol de técnico.</summary>
    public virtual ICollection<Ticket> TicketsAsignados { get; set; } = new List<Ticket>();

    /// <summary>Comentarios realizados por el usuario.</summary>
    public virtual ICollection<Comment> Comentarios { get; set; } = new List<Comment>();

    /// <summary>Historiales de cambio de estado realizados por el usuario.</summary>
    public virtual ICollection<StatusHistory> HistorialEstados { get; set; } = new List<StatusHistory>();

    /// <summary>Equipos en los que participa el usuario como técnico.</summary>
    public virtual ICollection<Team> Equipos { get; set; } = new List<Team>();
}

/// <summary>
/// Usuario con rol de Empleado, encargado de crear tickets de soporte.
/// </summary>
public class Empleado : User
{
    /// <summary>Inicializa una nueva instancia de <see cref="Empleado"/> con rol asignado.</summary>
    public Empleado()
    {
        Rol = UserRole.Empleado;
    }
}

/// <summary>
/// Usuario con rol de Técnico, encargado de atender y resolver tickets.
/// </summary>
public class Tecnico : User
{
    /// <summary>Inicializa una nueva instancia de <see cref="Tecnico"/> con rol asignado.</summary>
    public Tecnico()
    {
        Rol = UserRole.Tecnico;
    }
}

/// <summary>
/// Usuario con rol de Supervisor, encargado de asignar y administrar tickets.
/// </summary>
public class Supervisor : User
{
    /// <summary>Inicializa una nueva instancia de <see cref="Supervisor"/> con rol asignado.</summary>
    public Supervisor()
    {
        Rol = UserRole.Supervisor;
    }
}