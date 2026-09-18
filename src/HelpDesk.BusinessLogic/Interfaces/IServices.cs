using HelpDesk.Shared.DTOs;

namespace HelpDesk.BusinessLogic.Interfaces;

public interface ITicketService
{
    Task<TicketResponseDTO> CreateAsync(TicketCreateDTO dto, Guid empleadoId);
    Task<TicketResponseDTO?> GetByIdAsync(Guid id, Guid usuarioId, string usuarioRol);
    Task<PagedResultDTO<TicketListDTO>> GetFilteredAsync(TicketFilterDTO filter, Guid usuarioId, string usuarioRol);
    Task<TicketResponseDTO> UpdateAsync(Guid id, TicketUpdateDTO dto, Guid usuarioId, string usuarioRol);
    Task<TicketResponseDTO> AssignTechnicianAsync(Guid id, TicketAssignDTO dto, Guid supervisorId);
    Task<TicketResponseDTO> ChangeStatusAsync(Guid id, TicketStatusDTO dto, Guid usuarioId, string usuarioRol);
    Task<TicketResponseDTO> ReopenAsync(Guid id, Guid supervisorId);
    Task DeleteAsync(Guid id, Guid supervisorId);
}

public interface ICommentService
{
    Task<CommentResponseDTO> CreateAsync(Guid ticketId, CommentCreateDTO dto, Guid usuarioId, string usuarioRol);
    Task<IEnumerable<CommentResponseDTO>> GetByTicketIdAsync(Guid ticketId, Guid usuarioId, string usuarioRol, bool soloPublicos = false);
}

public interface IUserService
{
    Task<UserResponseDTO> CreateAsync(UserCreateDTO dto);
    Task<UserResponseDTO?> GetByIdAsync(Guid id);
    Task<PagedResultDTO<UserListDTO>> GetFilteredAsync(UserFilterDTO filter);
    Task<UserResponseDTO> UpdateAsync(Guid id, UserUpdateDTO dto);
    Task DeactivateAsync(Guid id);
    Task<UserResponseDTO?> GetByEmailAsync(string email);
    Task<LoginResponseDTO> LoginAsync(LoginDTO dto);
}

public interface ICategoryService
{
    Task<CategoryResponseDTO> CreateAsync(CategoryCreateDTO dto);
    Task<CategoryResponseDTO?> GetByIdAsync(Guid id);
    Task<IEnumerable<CategoryListDTO>> GetAllAsync(bool soloActivos = true);
    Task<CategoryResponseDTO> UpdateAsync(Guid id, CategoryUpdateDTO dto);
    Task DeleteAsync(Guid id);
}

public interface IPriorityService
{
    Task<IEnumerable<PriorityResponseDTO>> GetAllAsync();
    Task<PriorityResponseDTO?> GetByIdAsync(Guid id);
}

public interface IStatusService
{
    Task<IEnumerable<StatusResponseDTO>> GetAllAsync();
    Task<StatusResponseDTO?> GetByIdAsync(Guid id);
    Task<StatusResponseDTO?> GetInitialStatusAsync();
    Task<StatusResponseDTO?> GetClosedStatusAsync();
}

public interface ITeamService
{
    Task<TeamResponseDTO> CreateAsync(TeamCreateDTO dto);
    Task<TeamResponseDTO?> GetByIdAsync(Guid id);
    Task<PagedResultDTO<TeamListDTO>> GetFilteredAsync(TeamFilterDTO filter);
    Task<TeamResponseDTO> UpdateAsync(Guid id, TeamUpdateDTO dto);
    Task DeleteAsync(Guid id);
    Task<TeamResponseDTO> AddTecnicoAsync(Guid id, TeamAddTecnicoDTO dto);
    Task<TeamResponseDTO> RemoveTecnicoAsync(Guid id, TeamRemoveTecnicoDTO dto);
}

public interface IReportService
{
    Task<DashboardStatsDTO> GetDashboardStatsAsync();
    Task<IEnumerable<TicketsByStatusDTO>> GetTicketsByStatusAsync();
    Task<IEnumerable<TicketsByPriorityDTO>> GetTicketsByPriorityAsync();
    Task<IEnumerable<TicketsByTechnicianDTO>> GetTicketsByTechnicianAsync();
    Task<IEnumerable<SLAComplianceDTO>> GetSLAComplianceAsync();
    Task<IEnumerable<TechnicianWorkloadDTO>> GetTechnicianWorkloadAsync();
}