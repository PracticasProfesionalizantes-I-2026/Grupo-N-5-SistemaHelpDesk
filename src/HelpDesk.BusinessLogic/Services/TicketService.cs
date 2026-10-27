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
    private readonly INotificationService? _notificationService;

    public TicketService(
        ITicketRepository ticketRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        IPriorityRepository priorityRepository,
        IStatusRepository statusRepository,
        ICommentRepository commentRepository,
        IStatusHistoryRepository statusHistoryRepository,
        ITeamRepository teamRepository,
        IRatingRepository ratingRepository,
        INotificationService? notificationService = null
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
        _notificationService = notificationService;
    }

    public async Task<TicketResponseDTO> CreateAsync(TicketCreateDTO dto, Guid empleadoId)
    {
        if (dto == null)
            throw new ValidationException("Los datos para la creación del ticket son obligatorios");

        if (empleadoId == Guid.Empty)
            throw new ValidationException("El identificador del empleado no es válido");

        if (string.IsNullOrWhiteSpace(dto.Titulo))
            throw new ValidationException(ErrorMessages.TicketTitleRequired);

        if (dto.Titulo.Length > 200)
            throw new ValidationException("El título no puede superar los 200 caracteres");

        if (string.IsNullOrWhiteSpace(dto.Descripcion))
            throw new ValidationException(ErrorMessages.TicketDescriptionRequired);

        if (dto.Descripcion.Length > 5000)
            throw new ValidationException("La descripción no puede superar los 5000 caracteres");

        if (dto.PrioridadId == null || dto.PrioridadId == Guid.Empty)
            throw new ValidationException("Debe seleccionar una prioridad válida");

        if (dto.CategoriaId == null || dto.CategoriaId == Guid.Empty)
            throw new ValidationException("Debe seleccionar una categoría válida");

        await ValidateUserExistsAsync(empleadoId);
        await ValidateCategoryExistsAsync(dto.CategoriaId.Value);
        await ValidatePriorityExistsAsync(dto.PrioridadId.Value);

        var initialStatus = await _statusRepository.GetInitialStatusAsync();
        if (initialStatus == null)
            throw new BusinessRuleException("No hay estado inicial configurado");

        var ticket = new Ticket
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            PrioridadId = dto.PrioridadId!.Value,
            EstadoId = initialStatus.Id,
            
            CategoriaId = dto.CategoriaId!.Value,
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

        if (dto.Titulo != null)
        {
            if (string.IsNullOrWhiteSpace(dto.Titulo))
                throw new ValidationException(ErrorMessages.TicketTitleRequired);
            ticket.Titulo = dto.Titulo;
        }

        if (dto.Descripcion != null)
        {
            if (string.IsNullOrWhiteSpace(dto.Descripcion))
                throw new ValidationException(ErrorMessages.TicketDescriptionRequired);
            ticket.Descripcion = dto.Descripcion;
        }
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
        if (id == Guid.Empty)
            throw new ValidationException("El identificador del ticket no es válido");

        if (dto == null)
            throw new ValidationException("Los datos de asignación son obligatorios");

        if (dto.TecnicoId == Guid.Empty)
            throw new ValidationException("El identificador del técnico no es válido");

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

        if (_notificationService != null)
        {
            await _notificationService.EnviarNotificacionAsync(
                ticket.Id,
                dto.TecnicoId,
                "Nuevo ticket asignado",
                $"Se le ha asignado el ticket #{ticket.Id}: '{ticket.Titulo}'");

            if (ticket.EmpleadoId != Guid.Empty)
            {
                await _notificationService.EnviarNotificacionAsync(
                    ticket.Id,
                    ticket.EmpleadoId,
                    "Ticket asignado",
                    $"Su ticket #{ticket.Id} ha sido asignado a un técnico de soporte.");
            }
        }

        return await MapToResponseDTO(ticket);
    }

    public async Task<TicketResponseDTO> ChangeStatusAsync(Guid id, TicketStatusDTO dto, Guid usuarioId, string usuarioRol)
    {
        if (id == Guid.Empty)
            throw new ValidationException("El identificador del ticket no es válido");

        if (dto == null)
            throw new ValidationException("Los datos para cambio de estado son obligatorios");

        if (dto.EstadoId == Guid.Empty)
            throw new ValidationException("El identificador del estado no es válido");

        if (dto.Observacion != null && dto.Observacion.Length > 1000)
            throw new ValidationException("La observación no puede superar los 1000 caracteres");

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

        if (_notificationService != null && ticket.EmpleadoId != Guid.Empty)
        {
            await _notificationService.EnviarNotificacionAsync(
                ticket.Id,
                ticket.EmpleadoId,
                $"Ticket {newStatus.Nombre}",
                $"El estado de su ticket #{ticket.Id} ha cambiado a '{newStatus.Nombre}'.");
        }

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

            if (_notificationService != null)
            {
                var supervisors = await _userRepository.GetByRoleAsync("Supervisor");
                foreach (var sup in supervisors.Where(s => s.Activo))
                {
                    await _notificationService.EnviarNotificacionAsync(
                        ticketDb.Id,
                        sup.Id,
                        "Alerta SLA: Ticket Escalado",
                        $"El ticket #{ticketDb.Id} '{ticketDb.Titulo}' ha superado el SLA y fue escalado.");
                }
            }

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
        if (id == Guid.Empty)
            throw new ValidationException("El identificador del ticket no es válido");

        if (userRole != UserRole.Supervisor.ToString())
            throw new UnauthorizedActionException("Solo un Supervisor puede eliminar tickets.");

        var ticket = await _ticketRepository.GetByIdAsync(id, asNoTracking: false);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        var initialStatus = await _statusRepository.GetInitialStatusAsync();
        if (initialStatus != null && ticket.EstadoId != initialStatus.Id)
            throw new BusinessRuleException("Solo se pueden eliminar tickets en estado 'Abierto'");
        else if (ticket.Estado != null && !ticket.Estado.Nombre.Equals("Abierto", StringComparison.OrdinalIgnoreCase))
            throw new BusinessRuleException("Solo se pueden eliminar tickets en estado 'Abierto'");

        await _ticketRepository.DeleteAsync(id);
    }


    public async Task DeleteAsync(Guid id, Guid supervisorId)
    {
        if (id == Guid.Empty)
            throw new ValidationException("El identificador del ticket no es válido");

        var supervisor = await _userRepository.GetByIdAsync(supervisorId);
        if (supervisor == null || supervisor.Rol != UserRole.Supervisor)
            throw new UnauthorizedActionException(ErrorMessages.OnlySupervisorCanAssign);

        var ticket = await _ticketRepository.GetByIdAsync(id, asNoTracking: false);
        if (ticket == null)
            throw new NotFoundException(ErrorMessages.TicketNotFound);

        var initialStatus = await _statusRepository.GetInitialStatusAsync();
        if (initialStatus != null && ticket.EstadoId != initialStatus.Id)
            throw new BusinessRuleException("Solo se pueden eliminar tickets en estado 'Abierto'");
        else if (ticket.Estado != null && !ticket.Estado.Nombre.Equals("Abierto", StringComparison.OrdinalIgnoreCase))
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
        if (ticketId == Guid.Empty)
            throw new ValidationException("El identificador del ticket no es válido");

        if (usuarioId == Guid.Empty)
            throw new ValidationException("El identificador del usuario no es válido");

        if (dto == null)
            throw new ValidationException("Los datos de la calificación son obligatorios");

        if (dto.Puntuacion < 1 || dto.Puntuacion > 5)
            throw new ValidationException("Puntuacion", "La calificación debe ser entre 1 y 5");

        if (dto.Comentario != null && dto.Comentario.Length > 1000)
            throw new ValidationException("El comentario no puede superar los 1000 caracteres");

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
            ticket.Descripcion,
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
    public async Task ResolveAsync(Guid ticketId, string userRole, Guid usuarioId, bool cerrar)
    {
        if (userRole != "Supervisor")
            throw new UnauthorizedAccessException("Only Supervisors can resolve tickets.");

        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
            throw new KeyNotFoundException("Ticket not found.");

        ticket.FechaResolucion = DateTime.UtcNow;

        if (cerrar)
        {
            var closedStatus = await _statusRepository.GetClosedStatusAsync(true);
            if (closedStatus != null)
            {
                ticket.EstadoId = closedStatus.Id;
            }
        }

        await _ticketRepository.UpdateAsync(ticket);
    }



    public async Task AutoAssignTicketAsync()
    {
        var ticket = await _ticketRepository.GetUnassignedTicketAsync();
        if (ticket == null)
            return;

        // 1. Obtener equipos asociados a la categoría del ticket
        var teams = await _teamRepository.GetByCategoryAsync(ticket.CategoriaId);
        var activeTechnicianIds = new HashSet<Guid>();

        foreach (var team in teams)
        {
            var teamWithTechs = await _teamRepository.GetWithTecnicosAsync(team.Id);
            if (teamWithTechs != null)
            {
                foreach (var tech in teamWithTechs.Tecnicos)
                {
                    if (tech.Activo)
                    {
                        activeTechnicianIds.Add(tech.Id);
                    }
                }
            }
        }

        // Si no hay equipos específicos para la categoría, consultar los técnicos activos generales
        if (!activeTechnicianIds.Any())
        {
            var generalTechs = await _userRepository.GetActiveTechniciansAsync();
            foreach (var tech in generalTechs)
            {
                activeTechnicianIds.Add(tech.Id);
            }
        }

        // 2a. Si no hay técnicos disponibles, el ticket permanece sin asignación
        if (!activeTechnicianIds.Any())
        {
            return;
        }

        // 2. Analizar carga de trabajo de cada técnico (tickets asignados no finalizados)
        Guid bestTechnicianId = Guid.Empty;
        int minWorkload = int.MaxValue;

        // Orden por ID para criterio determinista / round-robin en caso de empate
        foreach (var techId in activeTechnicianIds.OrderBy(id => id))
        {
            var assignedTickets = await _ticketRepository.GetByTecnicoIdAsync(techId);
            var workload = assignedTickets.Count(t => t.Estado == null || !t.Estado.EsFinal);

            if (workload < minWorkload)
            {
                minWorkload = workload;
                bestTechnicianId = techId;
            }
        }

        if (bestTechnicianId == Guid.Empty)
            return;

        // 3. Asignar ticket al técnico seleccionado y actualizar estado a 'En Progreso'
        var inProgressStatus = await _statusRepository.GetByNameAsync("En Progreso");
        var oldStatusId = ticket.EstadoId;

        ticket.TecnicoId = bestTechnicianId;
        ticket.FechaAsignacion = DateTime.UtcNow;
        ticket.FechaActualizacion = DateTime.UtcNow;

        if (inProgressStatus != null)
        {
            ticket.EstadoId = inProgressStatus.Id;
            ticket.Estado = inProgressStatus;
        }

        await _ticketRepository.UpdateAsync(ticket);

        // 4. Registrar en historial
        var sistemaId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        await CreateStatusHistoryAsync(
            ticket.Id,
            oldStatusId,
            ticket.EstadoId,
            sistemaId,
            "Asignación automática por carga de trabajo y categoría");

        // 5. Notificar al técnico y al usuario creador (RF-10)
        if (_notificationService != null)
        {
            await _notificationService.EnviarNotificacionAsync(
                ticket.Id,
                bestTechnicianId,
                "Ticket asignado automáticamente",
                $"Se le ha asignado automáticamente el ticket #{ticket.Id}: '{ticket.Titulo}'");

            if (ticket.EmpleadoId != Guid.Empty)
            {
                await _notificationService.EnviarNotificacionAsync(
                    ticket.Id,
                    ticket.EmpleadoId,
                    "Ticket en progreso",
                    $"Su ticket #{ticket.Id} ha sido asignado y se encuentra en progreso.");
            }
        }
    }
}