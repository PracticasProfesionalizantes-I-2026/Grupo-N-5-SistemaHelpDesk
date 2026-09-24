namespace HelpDesk.DataAccess.Entities;

/// <summary>
/// Prioridad de atención de un ticket, con su nivel, color y SLA en horas.
/// </summary>
public class Priority
{
    /// <summary>Identificador único de la prioridad.</summary>
    public Guid Id { get; set; }

    /// <summary>Nombre de la prioridad.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Nivel de la prioridad (mayor = más importante).</summary>
    public int Nivel { get; set; }

    /// <summary>Color representativo de la prioridad.</summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>Horas permitidas para resolver según SLA.</summary>
    public int SLAHoras { get; set; }

    /// <summary>Versión de fila para concurrencia optimista.</summary>
    public byte[]? RowVersion { get; set; }

    /// <summary>Tickets asociados a la prioridad.</summary>
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}