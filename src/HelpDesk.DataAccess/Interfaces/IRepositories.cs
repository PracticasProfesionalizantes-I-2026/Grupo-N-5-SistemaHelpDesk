using HelpDesk.DataAccess.Entities;

namespace HelpDesk.DataAccess.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true);
    Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = true);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync();
}

public interface ITicketRepository : IRepository<Ticket>
{
    Task<IEnumerable<Ticket>> GetByEmpleadoIdAsync(Guid empleadoId, bool asNoTracking = true);
    Task<IEnumerable<Ticket>> GetByTecnicoIdAsync(Guid tecnicoId, bool asNoTracking = true);
    Task<IEnumerable<Ticket>> GetFilteredAsync(
        Guid? estadoId = null,
        Guid? prioridadId = null,
        Guid? categoriaId = null,
        Guid? tecnicoId = null,
        Guid? empleadoId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        int page = 1,
        int pageSize = 20,
        bool asNoTracking = true);
    Task<int> GetFilteredCountAsync(
        Guid? estadoId = null,
        Guid? prioridadId = null,
        Guid? categoriaId = null,
        Guid? tecnicoId = null,
        Guid? empleadoId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null);
    Task<IEnumerable<Ticket>> GetOverdueAsync(bool asNoTracking = true);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, bool asNoTracking = true);
    Task<IEnumerable<User>> GetByRoleAsync(string role, bool asNoTracking = true);
    Task<IEnumerable<User>> GetActiveTechniciansAsync(bool asNoTracking = true);
    Task<IEnumerable<User>> GetFilteredAsync(
        string? role = null,
        bool? activo = null,
        string? search = null,
        int page = 1,
        int pageSize = 20,
        bool asNoTracking = true);
    Task<int> GetFilteredCountAsync(
        string? role = null,
        bool? activo = null,
        string? search = null);
}

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string nombre, bool asNoTracking = true);
    Task<IEnumerable<Category>> GetActiveAsync(bool asNoTracking = true);
}

public interface IPriorityRepository : IRepository<Priority>
{
    Task<Priority?> GetByNameAsync(string nombre, bool asNoTracking = true);
    Task<IEnumerable<Priority>> GetAllOrderedAsync(bool asNoTracking = true);
}

public interface IStatusRepository : IRepository<Status>
{
    Task<Status?> GetByNameAsync(string nombre, bool asNoTracking = true);
    Task<IEnumerable<Status>> GetAllOrderedAsync(bool asNoTracking = true);
    Task<Status?> GetInitialStatusAsync(bool asNoTracking = true);
    Task<Status?> GetClosedStatusAsync(bool asNoTracking = true);
}

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByTicketIdAsync(Guid ticketId, bool asNoTracking = true);
    Task<IEnumerable<Comment>> GetPublicByTicketIdAsync(Guid ticketId, bool asNoTracking = true);
}

public interface IStatusHistoryRepository : IRepository<StatusHistory>
{
    Task<IEnumerable<StatusHistory>> GetByTicketIdAsync(Guid ticketId, bool asNoTracking = true);
    Task<StatusHistory> CreateAsync(StatusHistory entity);
}