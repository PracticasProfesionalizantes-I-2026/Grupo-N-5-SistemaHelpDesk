using HelpDesk.Shared.Enums;

namespace HelpDesk.DataAccess.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public Guid PrioridadId { get; set; }
    public Guid EstadoId { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid EmpleadoId { get; set; }
    public Guid? TecnicoId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public byte[]? RowVersion { get; set; }
    
    public virtual Priority Prioridad { get; set; } = null!;
    public virtual Status Estado { get; set; } = null!;
    public virtual Category Categoria { get; set; } = null!;
    public virtual User Empleado { get; set; } = null!;
    public virtual User? Tecnico { get; set; }
    public virtual ICollection<Comment> Comentarios { get; set; } = new List<Comment>();
    public virtual ICollection<StatusHistory> HistorialEstados { get; set; } = new List<StatusHistory>();
}