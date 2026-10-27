namespace HelpDesk.Shared.DTOs;

using System.ComponentModel.DataAnnotations;

public record ForgotPasswordDTO(
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    string? Email
);

public record ResetPasswordDTO(
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    string? Email,

    [Required(ErrorMessage = "El código de recuperación es obligatorio")]
    string? Codigo,

    [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    string? NuevaPassword
);

public record PasswordResetResponseDTO(
    bool Exitoso,
    string Mensaje,
    string? Codigo = null
);
