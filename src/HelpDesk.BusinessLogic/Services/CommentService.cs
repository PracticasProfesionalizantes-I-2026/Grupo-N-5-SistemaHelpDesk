using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Constants;

namespace HelpDesk.BusinessLogic.Services;

public class CommentService : BaseService, ICommentService
{
    public CommentService(
        ITicketRepository ticketRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        IPriorityRepository priorityRepository,
        IStatusRepository statusRepository,
        ICommentRepository commentRepository,
        IStatusHistoryRepository statusHistoryRepository,
        ITeamRepository teamRepository,
        IRatingRepository ratingRepository)
        : base(ticketRepository, userRepository, categoryRepository, priorityRepository, statusRepository, commentRepository, statusHistoryRepository, teamRepository, ratingRepository)
    {
    }

    public async Task<CommentResponseDTO> CreateAsync(Guid ticketId, CommentCreateDTO dto, Guid usuarioId, string usuarioRol)
    {
        if (ticketId == Guid.Empty)
            throw new ValidationException("El identificador del ticket no es válido");

        if (usuarioId == Guid.Empty)
            throw new ValidationException("El identificador del usuario no es válido");

        if (dto == null)
            throw new ValidationException("Los datos del comentario son obligatorios");

        if (string.IsNullOrWhiteSpace(dto.Contenido))
            throw new ValidationException("El contenido del comentario es obligatorio");

        if (dto.Contenido.Length > 2000)
            throw new ValidationException("El comentario no puede superar los 2000 caracteres");

        await ValidateUserExistsAsync(usuarioId);

        var ticket = await _ticketRepository.GetByIdAsync(ticketId, asNoTracking: false);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        ValidateCanAccessTicket(ticket, usuarioId, usuarioRol);
        ValidateTicketNotClosed(ticket);

        var usuario = await _userRepository.GetByIdAsync(usuarioId);
        var esInterno = dto.EsInterno && (usuarioRol == UserRole.Tecnico.ToString() || usuarioRol == UserRole.Supervisor.ToString());

        var comment = new Comment
        {
            TicketId = ticketId,
            UsuarioId = usuarioId,
            Contenido = dto.Contenido,
            EsInterno = esInterno,
            FechaCreacion = DateTime.UtcNow
        };

        var created = await _commentRepository.CreateAsync(comment);

        return MapToResponseDTO(created, usuario!);
    }

    public async Task<IEnumerable<CommentResponseDTO>> GetByTicketIdAsync(Guid ticketId, Guid usuarioId, string usuarioRol, bool soloPublicos = false)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        ValidateCanAccessTicket(ticket, usuarioId, usuarioRol);

        IEnumerable<Comment> comments;
        if (soloPublicos || usuarioRol == UserRole.Empleado.ToString())
        {
            comments = await _commentRepository.GetPublicByTicketIdAsync(ticketId);
        }
        else
        {
            comments = await _commentRepository.GetByTicketIdAsync(ticketId);
        }

        var result = new List<CommentResponseDTO>();
        foreach (var comment in comments)
        {
            var usuario = await _userRepository.GetByIdAsync(comment.UsuarioId);
            if (usuario != null)
                result.Add(MapToResponseDTO(comment, usuario));
        }

        return result;
    }

    private CommentResponseDTO MapToResponseDTO(Comment comment, User usuario)
    {
        return new CommentResponseDTO(
            comment.Id,
            comment.Contenido,
            comment.EsInterno,
            new UserSummaryDTO(usuario.Id, usuario.NombreCompleto, usuario.Email, usuario.Rol.ToString()),
            comment.FechaCreacion
        );
    }
}