using HelpDesk.Shared.Enums;

namespace HelpDesk.DataAccess.Entities;

public abstract class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public UserRole Rol { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Activo { get; set; } = true;
    
    public virtual ICollection<Ticket> TicketsCreados { get; set; } = new List<Ticket>();
    public virtual ICollection<Ticket> TicketsAsignados { get; set; } = new List<Ticket>();
    public virtual ICollection<Comment> Comentarios { get; set; } = new List<Comment>();
    public virtual ICollection<StatusHistory> HistorialEstados { get; set; } = new List<StatusHistory>();
    public virtual ICollection<Team> Equipos { get; set; } = new List<Team>();
}

public class Empleado : User
{
    public Empleado()
    {
        Rol = UserRole.Empleado;
    }
}

public class Tecnico : User
{
    public Tecnico()
    {
        Rol = UserRole.Tecnico;
    }
}

public class Supervisor : User
{
    public Supervisor()
    {
        Rol = UserRole.Supervisor;
    }
}