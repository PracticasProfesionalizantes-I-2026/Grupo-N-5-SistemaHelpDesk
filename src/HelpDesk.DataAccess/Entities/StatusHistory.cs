namespace HelpDesk.DataAccess.Entities;

public class StatusHistory
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Guid? EstadoAnteriorId { get; set; }
    public Guid EstadoNuevoId { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime FechaCambio { get; set; }
    public string? Observacion { get; set; }
    public byte[]? RowVersion { get; set; }
    
    public virtual Ticket Ticket { get; set; } = null!;
    public virtual Status? EstadoAnterior { get; set; }
    public virtual Status EstadoNuevo { get; set; } = null!;
    public virtual User Usuario { get; set; } = null!;
}