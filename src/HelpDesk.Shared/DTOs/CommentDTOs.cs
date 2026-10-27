namespace HelpDesk.Shared.DTOs;

using System.ComponentModel.DataAnnotations;

public record CommentCreateDTO(
    [Required(ErrorMessage = "El contenido del comentario es obligatorio")]
    [MinLength(1, ErrorMessage = "El comentario no puede estar vacío")]
    [MaxLength(3000, ErrorMessage = "El comentario no puede superar los 3000 caracteres")]
    string? Contenido,
    bool EsInterno = false
);

public record CommentResponseDTO(
    Guid Id,
    string Contenido,
    bool EsInterno,
    UserSummaryDTO Usuario,
    DateTime FechaCreacion
);