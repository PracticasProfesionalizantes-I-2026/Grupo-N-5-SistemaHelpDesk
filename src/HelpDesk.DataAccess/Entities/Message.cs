namespace HelpDesk.DataAccess.Entities;

public class Message
{
    /// <summary>Identificador único del mensaje.</summary>
    public Guid Id { get; set; }
    /// <summary>Identificador del usuario emisor.</summary>
    public Guid SenderId { get; set; }
    /// <summary>Identificador del usuario receptor.</summary>
    public Guid ReceiverId { get; set; }
    /// <summary>Contenido del mensaje.</summary>
    public string Text { get; set; } = string.Empty;
    /// <summary>Fecha y hora en que se envió el mensaje.</summary>
    public DateTime SentAt { get; set; }
    /// <summary>Usuario emisor.</summary>
    public virtual User Sender { get; set; } = null!;
    /// <summary>Usuario receptor.</summary>
    public virtual User Receiver { get; set; } = null!;
}
