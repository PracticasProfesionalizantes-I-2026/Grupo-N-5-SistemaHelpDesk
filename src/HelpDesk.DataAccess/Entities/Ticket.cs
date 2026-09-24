using HelpDesk.Shared.Enums;

namespace HelpDesk.DataAccess.Entities;

/// <summary>
/// Ticket de soporte técnico con su prioridad, estado, categoría y usuarios involucrados.
/// </summary>
public class Ticket
{
    /// <summary>Identificador único del ticket.</summary>
    public Guid Id { get; set; }

    /// <summary>Título del ticket.</summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>Descripción del problema reportado.</summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Identificador de la prioridad del ticket.</summary>
    public Guid PrioridadId { get; set; }

    /// <summary>Identificador del estado actual del ticket.</summary>
    public Guid EstadoId { get; set; }

    /// <summary>Identificador de la categoría del ticket.</summary>
    public Guid CategoriaId { get; set; }

    /// <summary>Identificador del empleado que creó el ticket.</summary>
    public Guid EmpleadoId { get; set; }

    /// <summary>Identificador del técnico asignado al ticket (puede ser nulo).</summary>
    public Guid? TecnicoId { get; set; }

    /// <summary>Identificador del equipo asignado al ticket (puede ser nulo).</summary>
    public Guid? TeamId { get; set; }

    /// <summary>Fecha y hora de creación del ticket.</summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>Fecha y hora de la última actualización del ticket.</summary>
    public DateTime FechaActualizacion { get; set; }

    /// <summary>Fecha y hora en que el ticket fue resuelto (puede ser nula).</summary>
    public DateTime? FechaResolucion { get; set; }

    /// <summary>Fecha y hora en que el ticket fue cerrado (puede ser nula).</summary>
    public DateTime? FechaCierre { get; set; }

    /// <summary>Versión de fila para concurrencia optimista.</summary>
    public byte[]? RowVersion { get; set; }

    /// <summary>Prioridad asociada al ticket.</summary>
    public virtual Priority Prioridad { get; set; } = null!;

    /// <summary>Estado actual del ticket.</summary>
    public virtual Status Estado { get; set; } = null!;

    /// <summary>Categoría del ticket.</summary>
    public virtual Category Categoria { get; set; } = null!;

    /// <summary>Empleado que creó el ticket.</summary>
    public virtual User Empleado { get; set; } = null!;

    /// <summary>Técnico asignado al ticket.</summary>
    public virtual User? Tecnico { get; set; }

    /// <summary>Equipo asignado al ticket.</summary>
    public virtual Team? Team { get; set; }

    /// <summary>Comentarios asociados al ticket.</summary>
    public virtual ICollection<Comment> Comentarios { get; set; } = new List<Comment>();

    /// <summary>Historial de cambios de estado del ticket.</summary>
    public virtual ICollection<StatusHistory> HistorialEstados { get; set; } = new List<StatusHistory>();
}