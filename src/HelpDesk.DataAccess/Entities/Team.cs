namespace HelpDesk.DataAccess.Entities;

/// <summary>
/// Equipo de trabajo que agrupa técnicos para atender tickets de una categoría.
/// </summary>
public class Team
{
    /// <summary>Identificador único del equipo.</summary>
    public Guid Id { get; set; }

    /// <summary>Nombre del equipo.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción del equipo.</summary>
    public string? Descripcion { get; set; }

    /// <summary>Identificador de la categoría que atiende el equipo.</summary>
    public Guid CategoriaId { get; set; }

    /// <summary>Fecha y hora de creación del equipo.</summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>Indica si el equipo se encuentra activo.</summary>
    public bool Activo { get; set; } = true;

    /// <summary>Versión de fila para concurrencia optimista.</summary>
    public byte[]? RowVersion { get; set; }

    /// <summary>Categoría que atiende el equipo.</summary>
    public virtual Category Categoria { get; set; } = null!;

    /// <summary>Técnicos que integran el equipo.</summary>
    public virtual ICollection<User> Tecnicos { get; set; } = new List<User>();

    /// <summary>Tickets asignados al equipo.</summary>
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}