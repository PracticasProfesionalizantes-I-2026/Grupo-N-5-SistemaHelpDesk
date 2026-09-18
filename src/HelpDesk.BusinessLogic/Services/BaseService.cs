using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Constants;

namespace HelpDesk.BusinessLogic.Services;

public abstract class BaseService
{
    protected readonly ITicketRepository _ticketRepository;
    protected readonly IUserRepository _userRepository;
    protected readonly ICategoryRepository _categoryRepository;
    protected readonly IPriorityRepository _priorityRepository;
    protected readonly IStatusRepository _statusRepository;
    protected readonly ICommentRepository _commentRepository;
    protected readonly IStatusHistoryRepository _statusHistoryRepository;
    protected readonly ITeamRepository _teamRepository;

    protected BaseService(
        ITicketRepository ticketRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        IPriorityRepository priorityRepository,
        IStatusRepository statusRepository,
        ICommentRepository commentRepository,
        IStatusHistoryRepository statusHistoryRepository,
        ITeamRepository teamRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
        _priorityRepository = priorityRepository;
        _statusRepository = statusRepository;
        _commentRepository = commentRepository;
        _statusHistoryRepository = statusHistoryRepository;
        _teamRepository = teamRepository;
    }

    protected async Task ValidateUserExistsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException(ErrorMessages.UserNotFound);
        if (!user.Activo)
            throw new BusinessRuleException("El usuario está inactivo");
    }

    protected async Task ValidateTicketExistsAsync(Guid ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);
    }

    protected async Task ValidateCategoryExistsAsync(Guid categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category == null)
            throw new NotFoundException(ErrorMessages.CategoryNotFound);
        if (!category.Activo)
            throw new BusinessRuleException("La categoría está inactiva");
    }

    protected async Task ValidatePriorityExistsAsync(Guid priorityId)
    {
        var priority = await _priorityRepository.GetByIdAsync(priorityId);
        if (priority == null)
            throw new NotFoundException(ErrorMessages.PriorityNotFound);
    }

    protected async Task ValidateStatusExistsAsync(Guid statusId)
    {
        var status = await _statusRepository.GetByIdAsync(statusId);
        if (status == null)
            throw new NotFoundException(ErrorMessages.StatusNotFound);
    }

    protected async Task ValidateTechnicianAsync(Guid tecnicoId)
    {
        var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
        if (tecnico == null)
            throw new NotFoundException(ErrorMessages.UserNotFound);
        if (tecnico.Rol != UserRole.Tecnico)
            throw new BusinessRuleException(ErrorMessages.UserIsNotTechnician);
        if (!tecnico.Activo)
            throw new BusinessRuleException(ErrorMessages.TechnicianMustBeActive);
    }

    protected void ValidateTicketNotClosed(Ticket ticket)
    {
        if (ticket.Estado.EsFinal)
            throw new BusinessRuleException(ErrorMessages.CannotModifyClosedTicket);
    }

    protected void ValidateCanAccessTicket(Ticket ticket, Guid usuarioId, string usuarioRol)
    {
        var isSupervisor = usuarioRol == UserRole.Supervisor.ToString();
        var isEmpleado = ticket.EmpleadoId == usuarioId;
        var isTecnico = ticket.TecnicoId == usuarioId;

        if (!isSupervisor && !isEmpleado && !isTecnico)
            throw new UnauthorizedActionException(ErrorMessages.UnauthorizedAccess);
    }

    protected async Task<StatusHistory> CreateStatusHistoryAsync(
        Guid ticketId,
        Guid? estadoAnteriorId,
        Guid estadoNuevoId,
        Guid usuarioId,
        string? observacion = null)
    {
        var history = new StatusHistory
        {
            TicketId = ticketId,
            EstadoAnteriorId = estadoAnteriorId,
            EstadoNuevoId = estadoNuevoId,
            UsuarioId = usuarioId,
            FechaCambio = DateTime.UtcNow,
            Observacion = observacion
        };
        return await _statusHistoryRepository.CreateAsync(history);
    }
}