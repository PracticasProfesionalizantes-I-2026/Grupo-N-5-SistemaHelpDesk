using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Constants;

namespace HelpDesk.BusinessLogic.Services;

public class ReportService : IReportService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStatusRepository _statusRepository;
    private readonly IPriorityRepository _priorityRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ReportService(
        ITicketRepository ticketRepository,
        IUserRepository userRepository,
        IStatusRepository statusRepository,
        IPriorityRepository priorityRepository,
        ICategoryRepository categoryRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _statusRepository = statusRepository;
        _priorityRepository = priorityRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<DashboardStatsDTO> GetDashboardStatsAsync()
    {
        var allTickets = await _ticketRepository.GetAllAsync();
        var ticketsList = allTickets.ToList();

        var total = ticketsList.Count;
        var abiertos = ticketsList.Count(t => t.Estado.Nombre == "Abierto");
        var enProgreso = ticketsList.Count(t => t.Estado.Nombre == "En Progreso");
        var resueltos = ticketsList.Count(t => t.Estado.Nombre == "Resuelto");
        var cerrados = ticketsList.Count(t => t.Estado.Nombre == "Cerrado");

        var now = DateTime.UtcNow;
        var vencidos = ticketsList.Count(t => !t.Estado.EsFinal && 
            t.FechaCreacion.AddHours(BusinessRules.SLAHours.TryGetValue((TicketPriority)t.Prioridad.Nivel, out var h) ? h : 0) < now);

        var resueltosConFecha = ticketsList.Where(t => t.FechaResolucion.HasValue).ToList();
        var avgResolutionHours = resueltosConFecha.Any() 
            ? resueltosConFecha.Average(t => (t.FechaResolucion!.Value - t.FechaCreacion).TotalHours)
            : 0;

        var today = DateTime.UtcNow.Date;
        var creadosHoy = ticketsList.Count(t => t.FechaCreacion.Date == today);
        var resueltosHoy = ticketsList.Count(t => t.FechaResolucion.HasValue && t.FechaResolucion.Value.Date == today);

        return new DashboardStatsDTO(
            total, abiertos, enProgreso, resueltos, cerrados, vencidos,
            Math.Round(avgResolutionHours, 2),
            creadosHoy, resueltosHoy
        );
    }

    public async Task<IEnumerable<TicketsByStatusDTO>> GetTicketsByStatusAsync()
    {
        var allTickets = (await _ticketRepository.GetAllAsync()).ToList();
        var statuses = (await _statusRepository.GetAllAsync()).ToList();
        var total = allTickets.Count;

        return statuses.Select(s => new TicketsByStatusDTO(
            new StatusResponseDTO(s.Id, s.Nombre, s.Descripcion, s.EsFinal, s.Orden),
            allTickets.Count(t => t.EstadoId == s.Id),
            total > 0 ? Math.Round((double)allTickets.Count(t => t.EstadoId == s.Id) / total * 100, 2) : 0
        ));
    }

    public async Task<IEnumerable<TicketsByPriorityDTO>> GetTicketsByPriorityAsync()
    {
        var allTickets = (await _ticketRepository.GetAllAsync()).ToList();
        var priorities = (await _priorityRepository.GetAllAsync()).ToList();
        var total = allTickets.Count;

        return priorities.Select(p => new TicketsByPriorityDTO(
            new PriorityResponseDTO(p.Id, p.Nombre, p.Nivel, p.Color, p.SLAHoras),
            allTickets.Count(t => t.PrioridadId == p.Id),
            total > 0 ? Math.Round((double)allTickets.Count(t => t.PrioridadId == p.Id) / total * 100, 2) : 0
        ));
    }

    public async Task<IEnumerable<TicketsByTechnicianDTO>> GetTicketsByTechnicianAsync()
    {
        var technicians = (await _userRepository.GetByRoleAsync(UserRole.Tecnico.ToString())).ToList();
        var allTickets = (await _ticketRepository.GetAllAsync()).ToList();

        return technicians.Select(t =>
        {
            var assigned = allTickets.Where(tk => tk.TecnicoId == t.Id).ToList();
            var resueltos = assigned.Where(tk => tk.Estado.Nombre == "Resuelto" || tk.Estado.Nombre == "Cerrado").ToList();
            var avgHours = resueltos.Any() && resueltos.All(r => r.FechaResolucion.HasValue)
                ? resueltos.Average(r => (r.FechaResolucion!.Value - r.FechaCreacion).TotalHours)
                : 0;

            return new TicketsByTechnicianDTO(
                new UserSummaryDTO(t.Id, t.NombreCompleto, t.Email, t.Rol.ToString()),
                assigned.Count,
                assigned.Count(tk => tk.Estado.Nombre == "Abierto"),
                assigned.Count(tk => tk.Estado.Nombre == "En Progreso"),
                assigned.Count(tk => tk.Estado.Nombre == "Resuelto"),
                assigned.Count(tk => tk.Estado.Nombre == "Cerrado"),
                Math.Round(avgHours, 2)
            );
        });
    }

    public async Task<IEnumerable<SLAComplianceDTO>> GetSLAComplianceAsync()
    {
        var allTickets = (await _ticketRepository.GetAllAsync()).Where(t => t.Estado.EsFinal && t.FechaResolucion.HasValue).ToList();
        var priorities = (await _priorityRepository.GetAllAsync()).ToList();

        return priorities.Select(p =>
        {
            var priorityTickets = allTickets.Where(t => t.PrioridadId == p.Id).ToList();
            var total = priorityTickets.Count;
            var withinSLA = priorityTickets.Count(t => 
                (t.FechaResolucion!.Value - t.FechaCreacion).TotalHours <= p.SLAHoras);
            var overdue = total - withinSLA;
            var avgHours = total > 0 ? priorityTickets.Average(t => (t.FechaResolucion!.Value - t.FechaCreacion).TotalHours) : 0;

            return new SLAComplianceDTO(
                new PriorityResponseDTO(p.Id, p.Nombre, p.Nivel, p.Color, p.SLAHoras),
                total,
                withinSLA,
                overdue,
                total > 0 ? Math.Round((double)withinSLA / total * 100, 2) : 0,
                Math.Round(avgHours, 2)
            );
        });
    }

    public async Task<IEnumerable<TechnicianWorkloadDTO>> GetTechnicianWorkloadAsync()
    {
        var technicians = (await _userRepository.GetByRoleAsync(UserRole.Tecnico.ToString())).Where(t => t.Activo).ToList();
        var allTickets = (await _ticketRepository.GetAllAsync()).ToList();

        return technicians.Select(t =>
        {
            var activeTickets = allTickets.Where(tk => tk.TecnicoId == t.Id && !tk.Estado.EsFinal).ToList();
            var overdueTickets = activeTickets.Count(tk => 
                tk.FechaCreacion.AddHours(BusinessRules.SLAHours.TryGetValue((TicketPriority)tk.Prioridad.Nivel, out var h) ? h : 0) < DateTime.UtcNow);
            
            var resolvedThisMonth = allTickets.Count(tk => 
                tk.TecnicoId == t.Id && 
                tk.Estado.EsFinal && 
                tk.FechaResolucion.HasValue && 
                tk.FechaResolucion.Value.Month == DateTime.UtcNow.Month &&
                tk.FechaResolucion.Value.Year == DateTime.UtcNow.Year);

            var resolvedTickets = allTickets.Where(tk => 
                tk.TecnicoId == t.Id && 
                tk.Estado.EsFinal && 
                tk.FechaResolucion.HasValue).ToList();
            
            var avgHours = resolvedTickets.Any() 
                ? resolvedTickets.Average(r => (r.FechaResolucion!.Value - r.FechaCreacion).TotalHours)
                : 0;

            var workloadLevel = activeTickets.Count switch
            {
                0 => WorkloadLevel.Bajo,
                <= 5 => WorkloadLevel.Medio,
                <= 10 => WorkloadLevel.Alto,
                _ => WorkloadLevel.Critico
            };

            return new TechnicianWorkloadDTO(
                new UserSummaryDTO(t.Id, t.NombreCompleto, t.Email, t.Rol.ToString()),
                activeTickets.Count,
                overdueTickets,
                Math.Round(avgHours, 2),
                resolvedThisMonth,
                workloadLevel
            );
        });
    }
}