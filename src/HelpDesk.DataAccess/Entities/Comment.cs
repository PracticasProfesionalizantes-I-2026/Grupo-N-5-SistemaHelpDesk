namespace HelpDesk.DataAccess.Entities;

/// <summary>
/// Comentario asociado a un ticket, que puede ser público o interno.
/// </summary>
public class Comment
{
    /// <summary>Identificador único del comentario.</summary>
    public Guid Id { get; set; }

    /// <summary>Identificador del ticket al que pertenece el comentario.</summary>
    public Guid TicketId { get; set; }

    /// <summary>Identificador del usuario que creó el comentario.</summary>
    public Guid UsuarioId { get; set; }

    /// <summary>Contenido del comentario.</summary>
    public string Contenido { get; set; } = string.Empty;

    /// <summary>Indica si el comentario es interno (solo visible para Técnicos y Supervisores).</summary>
    public bool EsInterno { get; set; }

    /// <summary>Fecha y hora de creación del comentario.</summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>Versión de fila para concurrencia optimista.</summary>
    public byte[]? RowVersion { get; set; }

    /// <summary>Ticket al que pertenece el comentario.</summary>
    public virtual Ticket Ticket { get; set; } = null!;

    /// <summary>Usuario autor del comentario.</summary>
    public virtual User Usuario { get; set; } = null!;
}