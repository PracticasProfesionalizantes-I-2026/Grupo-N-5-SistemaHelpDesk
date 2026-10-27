namespace HelpDesk.Shared.DTOs;

using System.ComponentModel.DataAnnotations;

public record RatingCreateDTO(
    [Required(ErrorMessage = "La puntuación es obligatoria (valor entero entre 1 y 5)")]
    [Range(1, 5, ErrorMessage = "La puntuación debe ser un número entre 1 y 5")]
    int Puntuacion,

    [MaxLength(1000, ErrorMessage = "El comentario de la calificación no puede superar los 1000 caracteres")]
    string? Comentario = null
);

public record RatingResponseDTO(
    Guid Id,
    Guid TicketId,
    Guid UsuarioId,
    int Puntuacion,
    string? Comentario,
    DateTime FechaCreacion,
    string UsuarioNombre
);