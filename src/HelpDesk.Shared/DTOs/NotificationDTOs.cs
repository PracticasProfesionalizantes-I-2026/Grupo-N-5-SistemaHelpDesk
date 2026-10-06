namespace HelpDesk.Shared.DTOs;

public record NotificationResponseDTO(
    Guid Id,
    string Titulo,
    string Mensaje,
    bool Leida,
    DateTime FechaCreacion
);