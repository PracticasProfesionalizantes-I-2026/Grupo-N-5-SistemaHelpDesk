namespace HelpDesk.Shared.DTOs;

public record CommentCreateDTO(
    string Contenido,
    bool EsInterno = false
);

public record CommentResponseDTO(
    Guid Id,
    string Contenido,
    bool EsInterno,
    UserSummaryDTO Usuario,
    DateTime FechaCreacion
);