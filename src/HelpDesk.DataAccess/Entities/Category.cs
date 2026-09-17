namespace HelpDesk.DataAccess.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public byte[]? RowVersion { get; set; }
    
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}