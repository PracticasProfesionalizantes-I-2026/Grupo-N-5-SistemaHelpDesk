namespace HelpDesk.DataAccess.Entities;

public class Priority
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public string Color { get; set; } = string.Empty;
    public int SLAHoras { get; set; }
    public byte[]? RowVersion { get; set; }
    
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}