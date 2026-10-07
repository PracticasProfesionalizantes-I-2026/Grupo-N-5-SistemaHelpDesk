namespace HelpDesk.DataAccess.Entities;

public class Notification
{
    public Guid Id { get; set; }

    public Guid UsuarioId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Mensaje { get; set; } = string.Empty;

    public bool Leida { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual User Usuario { get; set; } = null!;
}