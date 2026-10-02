namespace HelpDesk.DataAccess.Entities;

/// <summary>Calificación de satisfacción del empleado sobre un ticket resuelto/cerrado.</summary>
public class Rating
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Guid UsuarioId { get; set; }
    public int Puntuacion { get; set; }  // 1 a 5
    public string? Comentario { get; set; }
    public DateTime FechaCreacion { get; set; }

    public virtual Ticket? Ticket { get; set; }
    public virtual User? Usuario { get; set; }
}