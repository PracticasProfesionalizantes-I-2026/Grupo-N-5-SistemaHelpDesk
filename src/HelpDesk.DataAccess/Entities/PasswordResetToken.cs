namespace HelpDesk.DataAccess.Entities;

/// <summary>
/// Código temporal para restablecer contraseña (un solo uso, con vencimiento).
/// </summary>
public class PasswordResetToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public DateTime ExpiraEn { get; set; }
    public bool Usado { get; set; }
    public DateTime FechaCreacion { get; set; }

    public virtual User? Usuario { get; set; }
}