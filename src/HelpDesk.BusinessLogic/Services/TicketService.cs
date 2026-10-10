using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.Constants;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Exceptions;

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
        IRatingRepository ratingRepository
    ) : base(
        ticketRepository,
        userRepository,
        categoryRepository,
        priorityRepository,
        statusRepository,
        commentRepository,
        statusHistoryRepository,
        teamRepository,
        ratingRepository
    )
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

        if (ticket.TecnicoId == dto.TecnicoId)
            throw new BusinessRuleException("El ticket ya está asignado a ese técnico");

        var tecnicoAnteriorId = ticket.TecnicoId;
        var oldStatusId = ticket.EstadoId;

        var inProgressStatus = await _statusRepository.GetByNameAsync("En Progreso");
        if (inProgressStatus == null)
            throw new BusinessRuleException("Estado 'En Progreso' no configurado");

        ticket.TecnicoId = dto.TecnicoId;
        ticket.EstadoId = inProgressStatus.Id;
        ticket.Estado = inProgressStatus;
        ticket.FechaActualizacion = DateTime.UtcNow;

        await _ticketRepository.UpdateAsync(ticket);

        string observacion;
        if (tecnicoAnteriorId.HasValue)
            observacion = $"Reasignado de técnico {tecnicoAnteriorId} a {dto.TecnicoId}";
        else
            observacion = "Asignado a técnico";

        await CreateStatusHistoryAsync(
            ticket.Id,
            oldStatusId,
            inProgressStatus.Id,
            supervisorId,
            observacion);

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

    public async Task UpdateStatusAsync(Guid ticketId, Guid estadoId)
    {
        await _ticketRepository.UpdateStatusAsync(ticketId, estadoId);
    }

    public async Task<int> EscalateOverdueTicketsAsync()
    {
        var escaladoStatus = await _statusRepository.GetByNameAsync("Escalado");
        if (escaladoStatus == null)
            throw new BusinessRuleException("Estado 'Escalado' no configurado. Revisar seed.");

        var tickets = await _ticketRepository.GetOverdueAsync();

        var ahora = DateTime.UtcNow;
        var escalados = 0;

        var sistemaId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        foreach (var ticket in tickets)
        {
            if (ticket.EstadoId == escaladoStatus.Id)
                continue;

            if (ticket.Estado?.EsFinal == true)
                continue;

            var prioridad = ticket.Prioridad
                ?? await _priorityRepository.GetByIdAsync(ticket.PrioridadId);

            if (prioridad == null)
                continue;

            var vencimiento = ticket.FechaCreacion.AddHours(prioridad.SLAHoras);
            if (ahora <= vencimiento)
                continue;

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
    public async Task DeleteTicketAsync(Guid id, string userRole)
    {
        if (userRole != "Supervisor")
            throw new UnauthorizedAccessException("Solo un Supervisor puede eliminar tickets.");

        await _ticketRepository.DeleteAsync(id);
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

    public async Task<IEnumerable<StatusHistoryResponseDTO>> GetHistoryAsync(Guid id, Guid usuarioId, string usuarioRol)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        ValidateCanAccessTicket(ticket, usuarioId, usuarioRol);

        var history = await _statusHistoryRepository.GetByTicketIdAsync(id);

        return history.Select(h => new StatusHistoryResponseDTO(
            h.Id,
            h.TicketId,
            h.EstadoAnteriorId,
            h.EstadoAnterior != null
                ? new StatusResponseDTO(h.EstadoAnterior.Id, h.EstadoAnterior.Nombre, h.EstadoAnterior.Descripcion, h.EstadoAnterior.EsFinal, h.EstadoAnterior.Orden)
                : null,
            new StatusResponseDTO(h.EstadoNuevo.Id, h.EstadoNuevo.Nombre, h.EstadoNuevo.Descripcion, h.EstadoNuevo.EsFinal, h.EstadoNuevo.Orden),
            new UserSummaryDTO(h.Usuario.Id, h.Usuario.NombreCompleto, h.Usuario.Email, h.Usuario.Rol.ToString()),
            h.FechaCambio,
            h.Observacion
        ));
    }

    public async Task<RatingResponseDTO> CreateRatingAsync(
        Guid ticketId,
        RatingCreateDTO dto,
        Guid usuarioId,
        string usuarioRol)
    {
        if (dto.Puntuacion < 1 || dto.Puntuacion > 5)
            throw new ValidationException("Puntuacion", "La calificación debe ser entre 1 y 5");

        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        if (ticket.EmpleadoId != usuarioId)
            throw new UnauthorizedActionException(ErrorMessages.UnauthorizedAccess);

        var estado = await _statusRepository.GetByIdAsync(ticket.EstadoId);
        var nombreEstado = estado?.Nombre ?? string.Empty;
        if (nombreEstado != "Resuelto" && nombreEstado != "Cerrado")
            throw new BusinessRuleException("Este ticket no está disponible para ser calificado");

        var existente = await _ratingRepository.GetByTicketIdAsync(ticketId);
        if (existente != null)
            throw new BusinessRuleException("Este ticket ya fue calificado");

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
            ""
        );
    }

    public async Task<RatingResponseDTO?> GetRatingByTicketIdAsync(
        Guid ticketId,
        Guid usuarioId,
        string usuarioRol)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        ValidateCanAccessTicket(ticket, usuarioId, usuarioRol);

        var rating = await _ratingRepository.GetByTicketIdAsync(ticketId);
        if (rating == null)
            return null;

        return new RatingResponseDTO(
            rating.Id,
            rating.TicketId,
            rating.UsuarioId,
            rating.Puntuacion,
            rating.Comentario,
            rating.FechaCreacion,
            ""
        );
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
    public async Task ResolveTicketAsync(Guid id, string userRole, bool cerrar = false)
    {
        if (userRole != "Supervisor")
            throw new UnauthorizedAccessException("Acceso restringido a Supervisores.");

        await _ticketRepository.ResolveAsync(id, cerrar);
    }
}