using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _repository;

    public MessageService(IMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task<MessageDTO> SendMessageAsync(Guid senderId, Guid receiverId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Message cannot be empty.");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = receiverId,
            Text = text,
            SentAt = DateTime.UtcNow
        };

        await _repository.AddAsync(message);

        return new MessageDTO(
            message.Id,
            message.SenderId,
            message.ReceiverId,
            message.Text,
            message.SentAt,
            new UserResponseDTO(
                message.Sender.Id,
                message.Sender.NombreCompleto,
                message.Sender.Email,
                message.Sender.Rol.ToString(),
                message.Sender.Activo,
                message.Sender.FechaCreacion
            ),
            new UserResponseDTO(
                message.Receiver.Id,
                message.Receiver.NombreCompleto,
                message.Receiver.Email,
                message.Receiver.Rol.ToString(),
                message.Receiver.Activo,
                message.Receiver.FechaCreacion
            )
        );
    }

    public async Task<IEnumerable<MessageDTO>> GetConversationAsync(Guid userId, Guid contactId)
    {
        var messages = await _repository.GetConversationAsync(userId, contactId);

        return messages.Select(m => new MessageDTO(
            m.Id,
            m.SenderId,
            m.ReceiverId,
            m.Text,
            m.SentAt,
            new UserResponseDTO(
                m.Sender.Id,
                m.Sender.Email,
                m.Sender.NombreCompleto,
                m.Sender.Rol.ToString(),
                m.Sender.Activo,
                m.Sender.FechaCreacion
            ),
            new UserResponseDTO(
                m.Receiver.Id,
                m.Receiver.Email,
                m.Receiver.NombreCompleto,
                m.Receiver.Rol.ToString(),
                m.Receiver.Activo,
                m.Receiver.FechaCreacion
            )
        ));
    }
}
