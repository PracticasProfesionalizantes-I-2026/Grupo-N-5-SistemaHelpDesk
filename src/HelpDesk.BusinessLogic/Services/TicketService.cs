using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Constants;

namespace HelpDesk.BusinessLogic.Services;

public class TicketService : BaseService, ITicketService
{
    public TicketService(
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

    public async Task<TicketResponseDTO> CreateAsync(TicketCreateDTO dto, Guid empleadoId)
    {
        await ValidateUserExistsAsync(empleadoId);
        await ValidateCategoryExistsAsync(dto.CategoriaId);
        await ValidatePriorityExistsAsync(dto.PrioridadId);

        var initialStatus = await _statusRepository.GetInitialStatusAsync();
        if (initialStatus == null)
            throw new BusinessRuleException("No hay estado inicial configurado");

        var ticket = new Ticket
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            PrioridadId = dto.PrioridadId,
            EstadoId = initialStatus.Id,
            Estado = initialStatus,
            CategoriaId = dto.CategoriaId,
            EmpleadoId = empleadoId,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        var created = await _ticketRepository.CreateAsync(ticket);

        await CreateStatusHistoryAsync(created.Id, null, initialStatus.Id, empleadoId, "Ticket creado");

        return await MapToResponseDTO(created);
    }

    public async Task<TicketResponseDTO?> GetByIdAsync(Guid id, Guid usuarioId, string usuarioRol)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null)
            return null;

        ValidateCanAccessTicket(ticket, usuarioId, usuarioRol);

        return await MapToResponseDTO(ticket);
    }

    public async Task<PagedResultDTO<TicketListDTO>> GetFilteredAsync(TicketFilterDTO filter, Guid usuarioId, string usuarioRol)
    {
        var isSupervisor = usuarioRol == UserRole.Supervisor.ToString();
        Guid? empleadoId = null;
        Guid? tecnicoId = null;

        if (!isSupervisor)
        {
            if (usuarioRol == UserRole.Empleado.ToString())
                empleadoId = usuarioId;
            else if (usuarioRol == UserRole.Tecnico.ToString())
                tecnicoId = usuarioId;
        }

        var items = await _ticketRepository.GetFilteredAsync(
            filter.EstadoId,
            filter.PrioridadId,
            filter.CategoriaId,
            tecnicoId ?? filter.TecnicoId,
            empleadoId ?? filter.EmpleadoId,
            filter.FechaDesde,
            filter.FechaHasta,
            filter.Page,
            filter.PageSize);

        var totalCount = await _ticketRepository.GetFilteredCountAsync(
            filter.EstadoId,
            filter.PrioridadId,
            filter.CategoriaId,
            tecnicoId ?? filter.TecnicoId,
            empleadoId ?? filter.EmpleadoId,
            filter.FechaDesde,
            filter.FechaHasta);

        var itemDTOs = new List<TicketListDTO>();
        foreach (var ticket in items)
        {
            itemDTOs.Add(MapToListDTO(ticket));
        }

        var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        return new PagedResultDTO<TicketListDTO>(itemDTOs, totalCount, filter.Page, filter.PageSize, totalPages);
    }

    public async Task<TicketResponseDTO> UpdateAsync(Guid id, TicketUpdateDTO dto, Guid usuarioId, string usuarioRol)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id, asNoTracking: false);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        ValidateCanAccessTicket(ticket, usuarioId, usuarioRol);
        ValidateTicketNotClosed(ticket);

        if (dto.Titulo != null) ticket.Titulo = dto.Titulo;
        if (dto.Descripcion != null) ticket.Descripcion = dto.Descripcion;
        if (dto.CategoriaId.HasValue)
        {
            await ValidateCategoryExistsAsync(dto.CategoriaId.Value);
            ticket.CategoriaId = dto.CategoriaId.Value;
        }
        if (dto.PrioridadId.HasValue)
        {
            await ValidatePriorityExistsAsync(dto.PrioridadId.Value);
            ticket.PrioridadId = dto.PrioridadId.Value;
        }

        ticket.FechaActualizacion = DateTime.UtcNow;

        await _ticketRepository.UpdateAsync(ticket);

        return await MapToResponseDTO(ticket);
    }

    public async Task<TicketResponseDTO> AssignTechnicianAsync(Guid id, TicketAssignDTO dto, Guid supervisorId)
    {
        var supervisor = await _userRepository.GetByIdAsync(supervisorId);
        if (supervisor == null || supervisor.Rol != UserRole.Supervisor)
            throw new UnauthorizedActionException(ErrorMessages.OnlySupervisorCanAssign);

        await ValidateTechnicianAsync(dto.TecnicoId);

        var ticket = await _ticketRepository.GetByIdAsync(id, asNoTracking: false);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        ValidateTicketNotClosed(ticket);

        if (ticket.TecnicoId.HasValue)
            throw new BusinessRuleException(ErrorMessages.TicketAlreadyAssigned);

        var oldStatusId = ticket.EstadoId;
        var inProgressStatus = await _statusRepository.GetByNameAsync("En Progreso");
        if (inProgressStatus == null)
            throw new BusinessRuleException("Estado 'En Progreso' no configurado");

        ticket.TecnicoId = dto.TecnicoId;
        ticket.EstadoId = inProgressStatus.Id;
        ticket.Estado = inProgressStatus;
        ticket.FechaActualizacion = DateTime.UtcNow;

        await _ticketRepository.UpdateAsync(ticket);
        await CreateStatusHistoryAsync(ticket.Id, oldStatusId, inProgressStatus.Id, supervisorId, $"Asignado a técnico");

        return await MapToResponseDTO(ticket);
    }

    public async Task<TicketResponseDTO> ChangeStatusAsync(Guid id, TicketStatusDTO dto, Guid usuarioId, string usuarioRol)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id, asNoTracking: false);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        ValidateCanAccessTicket(ticket, usuarioId, usuarioRol);
        ValidateTicketNotClosed(ticket);

        await ValidateStatusExistsAsync(dto.EstadoId);

        var newStatus = await _statusRepository.GetByIdAsync(dto.EstadoId);
        if (newStatus == null)
            throw new NotFoundException(ErrorMessages.StatusNotFound);

        var oldStatusId = ticket.EstadoId;
        ticket.EstadoId = dto.EstadoId;
        ticket.Estado = newStatus;
        ticket.FechaActualizacion = DateTime.UtcNow;

        if (newStatus.EsFinal)
        {
            ticket.FechaCierre = DateTime.UtcNow;
            if (newStatus.Nombre == "Resuelto")
                ticket.FechaResolucion = DateTime.UtcNow;
        }
        else
        {
            ticket.FechaCierre = null;
            if (newStatus.Nombre == "Resuelto")
                ticket.FechaResolucion = null;
        }

        await _ticketRepository.UpdateAsync(ticket);
        await CreateStatusHistoryAsync(ticket.Id, oldStatusId, dto.EstadoId, usuarioId, dto.Observacion);

        return await MapToResponseDTO(ticket);
    }
    public async Task<int> EscalateOverdueTicketsAsync()
{
    var escaladoStatus = await _statusRepository.GetByNameAsync("Escalado");
    if (escaladoStatus == null)
        throw new BusinessRuleException("Estado 'Escalado' no configurado. Revisar seed.");

    // Tickets que no están en estado final (usa el método que ya tengan)
    var tickets = await _ticketRepository.GetOverdueAsync();
    // Si GetOverdueAsync no existe o no trae bien, alternativa:
    // var tickets = await _ticketRepository.GetFilteredAsync(pageSize: 500);

    var ahora = DateTime.UtcNow;
    var escalados = 0;

    // Usuario "sistema" para el historial (supervisor del seed)
    var sistemaId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

    foreach (var ticket in tickets)
    {
        // Ya escalado → no tocar
        if (ticket.EstadoId == escaladoStatus.Id)
            continue;

        // Si viene marcado como final, saltear
        if (ticket.Estado?.EsFinal == true)
            continue;

        var prioridad = ticket.Prioridad
            ?? await _priorityRepository.GetByIdAsync(ticket.PrioridadId);

        if (prioridad == null)
            continue;

        var vencimiento = ticket.FechaCreacion.AddHours(prioridad.SLAHoras);
        if (ahora <= vencimiento)
            continue; // todavía dentro del SLA

        // Releer para update (tracking)
        var ticketDb = await _ticketRepository.GetByIdAsync(ticket.Id, asNoTracking: false);
        if (ticketDb == null)
            continue;

        if (ticketDb.EstadoId == escaladoStatus.Id)
            continue;

        var oldStatusId = ticketDb.EstadoId;
        ticketDb.EstadoId = escaladoStatus.Id;
        ticketDb.Estado = escaladoStatus;
        ticketDb.FechaActualizacion = ahora;

        await _ticketRepository.UpdateAsync(ticketDb);
        await CreateStatusHistoryAsync(
            ticketDb.Id,
            oldStatusId,
            escaladoStatus.Id,
            sistemaId,
            "Escalado automático por incumplimiento de SLA");

        escalados++;
    }

    return escalados;
}

    public async Task<TicketResponseDTO> ReopenAsync(Guid id, Guid supervisorId)
    {
        var supervisor = await _userRepository.GetByIdAsync(supervisorId);
        if (supervisor == null || supervisor.Rol != UserRole.Supervisor)
            throw new UnauthorizedActionException(ErrorMessages.OnlySupervisorCanReopen);

        var ticket = await _ticketRepository.GetByIdAsync(id, asNoTracking: false);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        var currentStatus = ticket.Estado ?? await _statusRepository.GetByIdAsync(ticket.EstadoId);
        if (currentStatus == null || !currentStatus.EsFinal)
            throw new BusinessRuleException("Solo se pueden reabrir tickets cerrados");

        var openStatus = await _statusRepository.GetByNameAsync("Abierto");
        if (openStatus == null)
            throw new BusinessRuleException("Estado 'Abierto' no configurado");

        var oldStatusId = ticket.EstadoId;
        ticket.EstadoId = openStatus.Id;
        ticket.Estado = openStatus;
        ticket.FechaActualizacion = DateTime.UtcNow;
        ticket.FechaCierre = null;
        ticket.FechaResolucion = null;

        await _ticketRepository.UpdateAsync(ticket);
        await CreateStatusHistoryAsync(ticket.Id, oldStatusId, openStatus.Id, supervisorId, "Ticket reabierto por supervisor");

        return await MapToResponseDTO(ticket);
    }

    public async Task DeleteAsync(Guid id, Guid supervisorId)
    {
        var supervisor = await _userRepository.GetByIdAsync(supervisorId);
        if (supervisor == null || supervisor.Rol != UserRole.Supervisor)
            throw new UnauthorizedActionException(ErrorMessages.OnlySupervisorCanAssign);

        var ticket = await _ticketRepository.GetByIdAsync(id, asNoTracking: false);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        if (ticket.EstadoId != (await _statusRepository.GetInitialStatusAsync())?.Id)
            throw new BusinessRuleException("Solo se pueden eliminar tickets en estado 'Abierto'");

        await _ticketRepository.DeleteAsync(id);
    }

    private async Task<TicketResponseDTO> MapToResponseDTO(Ticket ticket)
    {
        var prioridad = ticket.Prioridad ?? await _priorityRepository.GetByIdAsync(ticket.PrioridadId);
        var estado = ticket.Estado ?? await _statusRepository.GetByIdAsync(ticket.EstadoId);
        var categoria = ticket.Categoria ?? await _categoryRepository.GetByIdAsync(ticket.CategoriaId);
        var empleado = ticket.Empleado ?? await _userRepository.GetByIdAsync(ticket.EmpleadoId);
        User? tecnico = ticket.Tecnico;
        if (tecnico == null && ticket.TecnicoId.HasValue)
            tecnico = await _userRepository.GetByIdAsync(ticket.TecnicoId.Value);

        var comentarios = await _commentRepository.GetByTicketIdAsync(ticket.Id) ?? Enumerable.Empty<Comment>();
        var comentariosPublicos = comentarios.Count(c => !c.EsInterno);

        var slaHoras = prioridad != null && BusinessRules.SLAHours.TryGetValue((TicketPriority)prioridad.Nivel, out var horas) ? horas : 0;
        var esFinal = estado?.EsFinal ?? ticket.Estado?.EsFinal ?? false;
        var estaVencido = !esFinal && ticket.FechaCreacion.AddHours(slaHoras) < DateTime.UtcNow;

        return new TicketResponseDTO(
            ticket.Id,
            ticket.Titulo,
            ticket.Descripcion,
            new PriorityResponseDTO(prioridad?.Id ?? ticket.PrioridadId, prioridad?.Nombre ?? "", prioridad?.Nivel ?? 0, prioridad?.Color ?? "", prioridad?.SLAHoras ?? slaHoras),
            new StatusResponseDTO(estado?.Id ?? ticket.EstadoId, estado?.Nombre ?? "", estado?.Descripcion ?? "", estado?.EsFinal ?? false, estado?.Orden ?? 0),
            new CategoryResponseDTO(categoria?.Id ?? ticket.CategoriaId, categoria?.Nombre ?? "", categoria?.Descripcion ?? "", categoria?.Activo ?? true, 0),
            new UserSummaryDTO(empleado?.Id ?? ticket.EmpleadoId, empleado?.NombreCompleto ?? "", empleado?.Email ?? "", empleado?.Rol.ToString() ?? ""),
            tecnico != null ? new UserSummaryDTO(tecnico.Id, tecnico.NombreCompleto, tecnico.Email, tecnico.Rol.ToString()) : null,
            ticket.FechaCreacion,
            ticket.FechaActualizacion,
            ticket.FechaResolucion,
            ticket.FechaCierre,
            slaHoras,
            comentariosPublicos,
            estaVencido
        );
    }

    private TicketListDTO MapToListDTO(Ticket ticket)
    {
        var slaHoras = ticket.Prioridad != null && BusinessRules.SLAHours.TryGetValue((TicketPriority)ticket.Prioridad.Nivel, out var h) ? h : 0;
        var esFinal = ticket.Estado?.EsFinal ?? false;
        var estaVencido = !esFinal && ticket.FechaCreacion.AddHours(slaHoras) < DateTime.UtcNow;

        return new TicketListDTO(
            ticket.Id,
            ticket.Titulo,
            ticket.Prioridad != null ? new PriorityResponseDTO(ticket.Prioridad.Id, ticket.Prioridad.Nombre, ticket.Prioridad.Nivel, ticket.Prioridad.Color, ticket.Prioridad.SLAHoras) : null!,
            ticket.Estado != null ? new StatusResponseDTO(ticket.Estado.Id, ticket.Estado.Nombre, ticket.Estado.Descripcion, ticket.Estado.EsFinal, ticket.Estado.Orden) : null!,
            ticket.Categoria != null ? new CategoryResponseDTO(ticket.Categoria.Id, ticket.Categoria.Nombre, ticket.Categoria.Descripcion, ticket.Categoria.Activo, 0) : null!,
            ticket.Empleado != null ? new UserSummaryDTO(ticket.Empleado.Id, ticket.Empleado.NombreCompleto, ticket.Empleado.Email, ticket.Empleado.Rol.ToString()) : null!,
            ticket.Tecnico != null ? new UserSummaryDTO(ticket.Tecnico.Id, ticket.Tecnico.NombreCompleto, ticket.Tecnico.Email, ticket.Tecnico.Rol.ToString()) : null,
            ticket.FechaCreacion,
            ticket.FechaActualizacion,
            estaVencido
        );
    }

    public async Task<RatingResponseDTO> CreateRatingAsync(Guid ticketId, RatingCreateDTO dto, Guid usuarioId, string usuarioRol)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        ValidateCanAccessTicket(ticket, usuarioId, usuarioRol);

        var currentStatus = ticket.Estado ?? await _statusRepository.GetByIdAsync(ticket.EstadoId);
        if (currentStatus == null || !currentStatus.EsFinal)
            throw new BusinessRuleException("Solo se pueden calificar tickets cerrados o resueltos");

        if (ticket.EmpleadoId != usuarioId)
            throw new UnauthorizedActionException("Solo el creador del ticket puede calificarlo");

        var existingRating = await _ratingRepository.GetByTicketIdAsync(ticketId);
        if (existingRating != null)
            throw new BusinessRuleException("El ticket ya tiene una calificación");

        if (dto.Puntuacion < 1 || dto.Puntuacion > 5)
            throw new ValidationException("La puntuación debe estar entre 1 y 5");

        var user = await _userRepository.GetByIdAsync(usuarioId);
        if (user == null)
            throw new NotFoundException(ErrorMessages.UserNotFound);

        var rating = new Rating
        {
            TicketId = ticketId,
            UsuarioId = usuarioId,
            Puntuacion = dto.Puntuacion,
            Comentario = dto.Comentario,
            FechaCreacion = DateTime.UtcNow
        };

        var created = await _ratingRepository.CreateAsync(rating);

        return new RatingResponseDTO(
            created.Id,
            created.TicketId,
            created.UsuarioId,
            created.Puntuacion,
            created.Comentario,
            created.FechaCreacion,
            user.NombreCompleto);
    }

    public async Task<RatingResponseDTO?> GetRatingByTicketIdAsync(Guid ticketId, Guid usuarioId, string usuarioRol)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            return null;

        ValidateCanAccessTicket(ticket, usuarioId, usuarioRol);

        var rating = await _ratingRepository.GetByTicketIdAsync(ticketId);
        if (rating == null)
            return null;

        var user = await _userRepository.GetByIdAsync(rating.UsuarioId);
        if (user == null)
            return null;

        return new RatingResponseDTO(
            rating.Id,
            rating.TicketId,
            rating.UsuarioId,
            rating.Puntuacion,
            rating.Comentario,
            rating.FechaCreacion,
            user.NombreCompleto);
    }
}