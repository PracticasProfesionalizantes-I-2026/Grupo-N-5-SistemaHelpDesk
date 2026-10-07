namespace HelpDesk.Shared.DTOs;

public record RatingCreateDTO(
    int Puntuacion,
    string? Comentario = null
);

public record RatingResponseDTO(
    Guid Id,
    Guid TicketId,
    Guid UsuarioId,
    int Puntuacion,
    string? Comentario,
    DateTime FechaCreacion,
    string UsuarioNombre          // ← agregar esto
);