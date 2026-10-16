namespace HelpDesk.Shared.DTOs;

public record ForgotPasswordDTO(
    string Email
);

public record ResetPasswordDTO(
    string Email,
    string Codigo,
    string NuevaPassword
);

public record PasswordResetResponseDTO(
    bool Exitoso,
    string Mensaje,
    string? Codigo = null
);
