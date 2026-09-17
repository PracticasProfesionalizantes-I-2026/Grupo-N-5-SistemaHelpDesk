namespace HelpDesk.DataAccess.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public bool EsInterno { get; set; }
    public DateTime FechaCreacion { get; set; }
    public byte[]? RowVersion { get; set; }
    
    public virtual Ticket Ticket { get; set; } = null!;
    public virtual User Usuario { get; set; } = null!;
}