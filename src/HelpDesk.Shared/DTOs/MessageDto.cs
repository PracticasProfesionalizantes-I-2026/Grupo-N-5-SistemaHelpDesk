namespace HelpDesk.Shared.DTOs;

public record MessageDTO(
    Guid Id,
    Guid SenderId,
    Guid ReceiverId,
    string Text,
    DateTime SentAt,
    UserResponseDTO Sender,
    UserResponseDTO Receiver
);
