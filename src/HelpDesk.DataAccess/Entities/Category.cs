namespace HelpDesk.DataAccess.Entities;

/// <summary>
/// Categoría de soporte a la que se asocian tickets y equipos.
/// </summary>
public class Category
{
    /// <summary>Identificador único de la categoría.</summary>
    public Guid Id { get; set; }

    /// <summary>Nombre de la categoría.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción de la categoría.</summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Indica si la categoría se encuentra activa.</summary>
    public bool Activo { get; set; } = true;

    /// <summary>Versión de fila para concurrencia optimista.</summary>
    public byte[]? RowVersion { get; set; }

    /// <summary>Tickets asociados a la categoría.</summary>
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}