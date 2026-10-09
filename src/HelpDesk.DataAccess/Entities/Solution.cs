namespace HelpDesk.DataAccess.Entities;

/// <summary>
/// Solución técnica registrada por un técnico para un ticket.
/// </summary>
public class Solution
{
    /// <summary>Identificador único de la solución.</summary>
    public Guid Id { get; set; }

    /// <summary>Identificador del ticket al que pertenece la solución.</summary>
    public Guid TicketId { get; set; }

    /// <summary>Identificador del técnico que registró la solución.</summary>
    public Guid TecnicoId { get; set; }

    /// <summary>Descripción técnica de la solución aplicada.</summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Nombre original del archivo adjunto, si existe.</summary>
    public string? NombreArchivo { get; set; }

    /// <summary>Ruta o URL donde se encuentra almacenado el archivo.</summary>
    public string? RutaArchivo { get; set; }

    /// <summary>Tipo MIME/contenido del archivo adjunto.</summary>
    public string? TipoContenido { get; set; }

    /// <summary>Tamaño en bytes del archivo adjunto.</summary>
    public long? TamanioArchivo { get; set; }

    /// <summary>Contenido en bytes del archivo adjunto.</summary>
    public byte[]? ContenidoArchivo { get; set; }

    /// <summary>Fecha y hora en que se registró la solución.</summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>Ticket al que pertenece la solución.</summary>
    public virtual Ticket Ticket { get; set; } = null!;

    /// <summary>Técnico que registró la solución.</summary>
    public virtual User Tecnico { get; set; } = null!;
}