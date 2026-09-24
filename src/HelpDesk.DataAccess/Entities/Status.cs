namespace HelpDesk.DataAccess.Entities;

/// <summary>
/// Estado por el que transita un ticket dentro de su ciclo de vida.
/// </summary>
public class Status
{
    /// <summary>Identificador único del estado.</summary>
    public Guid Id { get; set; }

    /// <summary>Nombre del estado.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción del estado.</summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Indica si el estado es final (el ticket queda cerrado).</summary>
    public bool EsFinal { get; set; }

    /// <summary>Orden del estado dentro del flujo de trabajo.</summary>
    public int Orden { get; set; }

    /// <summary>Versión de fila para concurrencia optimista.</summary>
    public byte[]? RowVersion { get; set; }

    /// <summary>Tickets que se encuentran en este estado.</summary>
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    /// <summary>Historiales en los que este estado fue el anterior.</summary>
    public virtual ICollection<StatusHistory> HistorialEstadoAnterior { get; set; } = new List<StatusHistory>();

    /// <summary>Historiales en los que este estado fue el nuevo.</summary>
    public virtual ICollection<StatusHistory> HistorialEstadoNuevo { get; set; } = new List<StatusHistory>();
}