namespace HelpDesk.DataAccess.Entities;

public class Status
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool EsFinal { get; set; }
    public int Orden { get; set; }
    public byte[]? RowVersion { get; set; }
    
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public virtual ICollection<StatusHistory> HistorialEstadoAnterior { get; set; } = new List<StatusHistory>();
    public virtual ICollection<StatusHistory> HistorialEstadoNuevo { get; set; } = new List<StatusHistory>();
}