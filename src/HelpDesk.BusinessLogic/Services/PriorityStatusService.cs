using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;

namespace HelpDesk.BusinessLogic.Services;

public class PriorityService : IPriorityService
{
    private readonly IPriorityRepository _priorityRepository;

    public PriorityService(IPriorityRepository priorityRepository)
    {
        _priorityRepository = priorityRepository;
    }

    public async Task<IEnumerable<PriorityResponseDTO>> GetAllAsync()
    {
        var priorities = await _priorityRepository.GetAllOrderedAsync();
        return priorities.Select(p => new PriorityResponseDTO(p.Id, p.Nombre, p.Nivel, p.Color, p.SLAHoras));
    }

    public async Task<PriorityResponseDTO?> GetByIdAsync(Guid id)
    {
        var priority = await _priorityRepository.GetByIdAsync(id);
        if (priority == null)
            return null;

        return new PriorityResponseDTO(priority.Id, priority.Nombre, priority.Nivel, priority.Color, priority.SLAHoras);
    }
}

public class StatusService : IStatusService
{
    private readonly IStatusRepository _statusRepository;

    public StatusService(IStatusRepository statusRepository)
    {
        _statusRepository = statusRepository;
    }

    public async Task<IEnumerable<StatusResponseDTO>> GetAllAsync()
    {
        var statuses = await _statusRepository.GetAllOrderedAsync();
        return statuses.Select(s => new StatusResponseDTO(s.Id, s.Nombre, s.Descripcion, s.EsFinal, s.Orden));
    }

    public async Task<StatusResponseDTO?> GetByIdAsync(Guid id)
    {
        var status = await _statusRepository.GetByIdAsync(id);
        if (status == null)
            return null;

        return new StatusResponseDTO(status.Id, status.Nombre, status.Descripcion, status.EsFinal, status.Orden);
    }

    public async Task<StatusResponseDTO?> GetInitialStatusAsync()
    {
        var status = await _statusRepository.GetInitialStatusAsync();
        if (status == null)
            return null;

        return new StatusResponseDTO(status.Id, status.Nombre, status.Descripcion, status.EsFinal, status.Orden);
    }

    public async Task<StatusResponseDTO?> GetClosedStatusAsync()
    {
        var status = await _statusRepository.GetClosedStatusAsync();
        if (status == null)
            return null;

        return new StatusResponseDTO(status.Id, status.Nombre, status.Descripcion, status.EsFinal, status.Orden);
    }
}