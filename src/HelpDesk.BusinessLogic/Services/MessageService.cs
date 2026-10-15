using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.Constants;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;

namespace HelpDesk.BusinessLogic.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _repository;
    private readonly IUserRepository _userRepository;

    public MessageService(IMessageRepository repository, IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<MessageDTO> SendMessageAsync(Guid senderId, Guid receiverId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ValidationException("Text", "Message cannot be empty.");

        var sender = await _userRepository.GetByIdAsync(senderId)
            ?? throw new NotFoundException(ErrorMessages.UserNotFound);
        var receiver = await _userRepository.GetByIdAsync(receiverId)
            ?? throw new NotFoundException(ErrorMessages.UserNotFound);

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
                sender.Id,
                sender.Email,
                sender.NombreCompleto,
                sender.Rol.ToString(),
                sender.Activo,
                sender.FechaCreacion
            ),
            new UserResponseDTO(
                receiver.Id,
                receiver.Email,
                receiver.NombreCompleto,
                receiver.Rol.ToString(),
                receiver.Activo,
                receiver.FechaCreacion
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
