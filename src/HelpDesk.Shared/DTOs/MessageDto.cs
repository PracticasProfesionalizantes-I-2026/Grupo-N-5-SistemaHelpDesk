namespace HelpDesk.Shared.DTOs;

using System.ComponentModel.DataAnnotations;

public record SendMessageDTO(
    [Required(ErrorMessage = "El campo SenderId es obligatorio y debe ser un GUID válido")]
    Guid SenderId,

    [Required(ErrorMessage = "El campo ReceiverId es obligatorio y debe ser un GUID válido")]
    Guid ReceiverId,

    [Required(ErrorMessage = "El texto del mensaje es obligatorio")]
    [MinLength(1, ErrorMessage = "El mensaje no puede estar vacío")]
    [MaxLength(2000, ErrorMessage = "El mensaje no puede superar los 2000 caracteres")]
    string? Text
);

public record MessageDTO(
    Guid Id,
    Guid SenderId,
    Guid ReceiverId,
    string Text,
    DateTime SentAt,
    UserResponseDTO Sender,
    UserResponseDTO Receiver
);
