namespace HelpDesk.DataAccess.Entities;

public class Team
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public Guid CategoriaId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Activo { get; set; } = true;
    public byte[]? RowVersion { get; set; }
    
    public virtual Category Categoria { get; set; } = null!;
    public virtual ICollection<User> Tecnicos { get; set; } = new List<User>();
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}