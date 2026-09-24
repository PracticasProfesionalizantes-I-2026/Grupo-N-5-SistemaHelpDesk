namespace HelpDesk.DataAccess.Entities;

/// <summary>
/// Registro de cambio de estado de un ticket, con estado anterior y nuevo.
/// </summary>
public class StatusHistory
{
    /// <summary>Identificador único del registro de historial.</summary>
    public Guid Id { get; set; }

    /// <summary>Identificador del ticket cuyo estado cambió.</summary>
    public Guid TicketId { get; set; }

    /// <summary>Identificador del estado anterior (puede ser nulo si no existía).</summary>
    public Guid? EstadoAnteriorId { get; set; }

    /// <summary>Identificador del nuevo estado.</summary>
    public Guid EstadoNuevoId { get; set; }

    /// <summary>Identificador del usuario que realizó el cambio de estado.</summary>
    public Guid UsuarioId { get; set; }

    /// <summary>Fecha y hora del cambio de estado.</summary>
    public DateTime FechaCambio { get; set; }

    /// <summary>Observación asociada al cambio de estado.</summary>
    public string? Observacion { get; set; }

    /// <summary>Versión de fila para concurrencia optimista.</summary>
    public byte[]? RowVersion { get; set; }

    /// <summary>Ticket cuyo estado fue modificado.</summary>
    public virtual Ticket Ticket { get; set; } = null!;

    /// <summary>Estado anterior del ticket.</summary>
    public virtual Status? EstadoAnterior { get; set; }

    /// <summary>Nuevo estado del ticket.</summary>
    public virtual Status EstadoNuevo { get; set; } = null!;

    /// <summary>Usuario que realizó el cambio de estado.</summary>
    public virtual User Usuario { get; set; } = null!;
}