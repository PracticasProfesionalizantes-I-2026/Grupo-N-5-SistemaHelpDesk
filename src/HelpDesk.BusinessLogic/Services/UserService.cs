using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using HelpDesk.Shared.Enums;
using HelpDesk.Shared.Constants;

namespace HelpDesk.BusinessLogic.Services;

public class UserService : BaseService, IUserService
{
    public UserService(
        ITicketRepository ticketRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        IPriorityRepository priorityRepository,
        IStatusRepository statusRepository,
        ICommentRepository commentRepository,
        IStatusHistoryRepository statusHistoryRepository,
        ITeamRepository teamRepository)
        : base(ticketRepository, userRepository, categoryRepository, priorityRepository, statusRepository, commentRepository, statusHistoryRepository, teamRepository)
    {
    }

    public async Task<UserResponseDTO> CreateAsync(UserCreateDTO dto)
    {
        if (!Enum.TryParse<UserRole>(dto.Rol, out var rol))
            throw new ValidationException("Rol", "Rol inválido. Valores permitidos: Empleado, Tecnico, Supervisor");

        var existing = await _userRepository.GetByEmailAsync(dto.Email);
        if (existing != null)
            throw new DuplicateException(ErrorMessages.EmailAlreadyExists);

        var user = rol switch
        {
            UserRole.Empleado => (User)new Empleado(),
            UserRole.Tecnico => (User)new Tecnico(),
            UserRole.Supervisor => (User)new Supervisor(),
            _ => throw new ValidationException("Rol", "Rol inválido")
        };

        user.Email = dto.Email;
        user.NombreCompleto = dto.NombreCompleto;
        user.FechaCreacion = DateTime.UtcNow;
        user.Activo = true;

        var created = await _userRepository.CreateAsync(user);

        return MapToResponseDTO(created);
    }

    public async Task<UserResponseDTO?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        return MapToResponseDTO(user);
    }

    public async Task<PagedResultDTO<UserListDTO>> GetFilteredAsync(UserFilterDTO filter)
    {
        var items = await _userRepository.GetFilteredAsync(
            filter.Rol,
            filter.Activo,
            filter.Search,
            filter.Page,
            filter.PageSize);

        var totalCount = await _userRepository.GetFilteredCountAsync(
            filter.Rol,
            filter.Activo,
            filter.Search);

        var itemDTOs = new List<UserListDTO>();
        foreach (var user in items)
        {
            var ticketsAsignados = await _ticketRepository.GetByTecnicoIdAsync(user.Id);
            var ticketsCreados = await _ticketRepository.GetByEmpleadoIdAsync(user.Id);
            
            itemDTOs.Add(new UserListDTO(
                user.Id,
                user.Email,
                user.NombreCompleto,
                user.Rol.ToString(),
                user.Activo,
                user.FechaCreacion,
                ticketsAsignados.Count(),
                ticketsCreados.Count()
            ));
        }

        var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        return new PagedResultDTO<UserListDTO>(itemDTOs, totalCount, filter.Page, filter.PageSize, totalPages);
    }

    public async Task<UserResponseDTO> UpdateAsync(Guid id, UserUpdateDTO dto)
    {
        var user = await _userRepository.GetByIdAsync(id, asNoTracking: false);
        if (user == null)
            throw new NotFoundException(ErrorMessages.UserNotFound);

        if (dto.NombreCompleto != null)
            user.NombreCompleto = dto.NombreCompleto;

        if (dto.Rol != null)
        {
            if (!Enum.TryParse<UserRole>(dto.Rol, out var rol))
                throw new ValidationException("Rol", "Rol inválido");

            if (user.Rol == UserRole.Supervisor && rol != UserRole.Supervisor)
            {
                var ticketsAsignados = await _ticketRepository.GetByTecnicoIdAsync(user.Id);
                if (ticketsAsignados.Any(t => !t.Estado.EsFinal))
                    throw new BusinessRuleException("No se puede cambiar el rol de un supervisor con tickets asignados abiertos");
            }

            user.Rol = rol;
        }

        if (dto.Activo.HasValue)
        {
            if (!dto.Activo.Value)
            {
                var ticketsAbiertos = await _ticketRepository.GetByTecnicoIdAsync(user.Id);
                if (ticketsAbiertos.Any(t => !t.Estado.EsFinal))
                    throw new BusinessRuleException(ErrorMessages.UserHasOpenTickets);
            }
            user.Activo = dto.Activo.Value;
        }

        await _userRepository.UpdateAsync(user);

        return MapToResponseDTO(user);
    }

    public async Task DeactivateAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id, asNoTracking: false);
        if (user == null)
            throw new NotFoundException(ErrorMessages.UserNotFound);

        var ticketsAbiertos = await _ticketRepository.GetByTecnicoIdAsync(user.Id);
        if (ticketsAbiertos.Any(t => !t.Estado.EsFinal))
            throw new BusinessRuleException(ErrorMessages.UserHasOpenTickets);

        user.Activo = false;
        await _userRepository.UpdateAsync(user);
    }

    public async Task<UserResponseDTO?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return null;

        return MapToResponseDTO(user);
    }

    public async Task<LoginResponseDTO> LoginAsync(LoginDTO dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null)
            throw new UnauthorizedActionException(ErrorMessages.InvalidCredentials);

        if (!user.Activo)
            throw new UnauthorizedActionException("Usuario inactivo");

        var token = $"mock-token-{user.Id}-{DateTime.UtcNow.Ticks}"; // Simulación simple
        
        return new LoginResponseDTO(MapToResponseDTO(user), token);
    }

    private UserResponseDTO MapToResponseDTO(User user)
    {
        return new UserResponseDTO(
            user.Id,
            user.Email,
            user.NombreCompleto,
            user.Rol.ToString(),
            user.Activo,
            user.FechaCreacion
        );
    }
}