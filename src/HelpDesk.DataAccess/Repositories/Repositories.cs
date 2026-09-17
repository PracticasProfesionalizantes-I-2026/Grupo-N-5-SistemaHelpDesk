using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.DataAccess.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly HelpDeskDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(HelpDeskDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true)
    {
        var query = _dbSet.AsQueryable();
        if (asNoTracking)
            query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = true)
    {
        var query = _dbSet.AsQueryable();
        if (asNoTracking)
            query = query.AsNoTracking();
        return await query.ToListAsync();
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null && idProperty.PropertyType == typeof(Guid))
        {
            idProperty.SetValue(entity, Guid.NewGuid());
        }
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id, asNoTracking: false);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => EF.Property<Guid>(e, "Id") == id);
    }

    public virtual async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }
}

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(HelpDeskDbContext context) : base(context) { }

    public async Task<IEnumerable<Ticket>> GetByEmpleadoIdAsync(Guid empleadoId, bool asNoTracking = true)
    {
        var query = _dbSet.Where(t => t.EmpleadoId == empleadoId).Include(t => t.Prioridad).Include(t => t.Estado).Include(t => t.Categoria).Include(t => t.Empleado).Include(t => t.Tecnico);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.OrderByDescending(t => t.FechaCreacion).ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByTecnicoIdAsync(Guid tecnicoId, bool asNoTracking = true)
    {
        var query = _dbSet.Where(t => t.TecnicoId == tecnicoId).Include(t => t.Prioridad).Include(t => t.Estado).Include(t => t.Categoria).Include(t => t.Empleado).Include(t => t.Tecnico);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.OrderByDescending(t => t.FechaCreacion).ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetFilteredAsync(
        Guid? estadoId = null,
        Guid? prioridadId = null,
        Guid? categoriaId = null,
        Guid? tecnicoId = null,
        Guid? empleadoId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        int page = 1,
        int pageSize = 20,
        bool asNoTracking = true)
    {
        var query = _dbSet
            .Include(t => t.Prioridad)
            .Include(t => t.Estado)
            .Include(t => t.Categoria)
            .Include(t => t.Empleado)
            .Include(t => t.Tecnico)
            .AsQueryable();

        if (estadoId.HasValue) query = query.Where(t => t.EstadoId == estadoId.Value);
        if (prioridadId.HasValue) query = query.Where(t => t.PrioridadId == prioridadId.Value);
        if (categoriaId.HasValue) query = query.Where(t => t.CategoriaId == categoriaId.Value);
        if (tecnicoId.HasValue) query = query.Where(t => t.TecnicoId == tecnicoId.Value);
        if (empleadoId.HasValue) query = query.Where(t => t.EmpleadoId == empleadoId.Value);
        if (fechaDesde.HasValue) query = query.Where(t => t.FechaCreacion >= fechaDesde.Value);
        if (fechaHasta.HasValue) query = query.Where(t => t.FechaCreacion <= fechaHasta.Value);

        if (asNoTracking) query = query.AsNoTracking();

        return await query
            .OrderByDescending(t => t.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetFilteredCountAsync(
        Guid? estadoId = null,
        Guid? prioridadId = null,
        Guid? categoriaId = null,
        Guid? tecnicoId = null,
        Guid? empleadoId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null)
    {
        var query = _dbSet.AsQueryable();

        if (estadoId.HasValue) query = query.Where(t => t.EstadoId == estadoId.Value);
        if (prioridadId.HasValue) query = query.Where(t => t.PrioridadId == prioridadId.Value);
        if (categoriaId.HasValue) query = query.Where(t => t.CategoriaId == categoriaId.Value);
        if (tecnicoId.HasValue) query = query.Where(t => t.TecnicoId == tecnicoId.Value);
        if (empleadoId.HasValue) query = query.Where(t => t.EmpleadoId == empleadoId.Value);
        if (fechaDesde.HasValue) query = query.Where(t => t.FechaCreacion >= fechaDesde.Value);
        if (fechaHasta.HasValue) query = query.Where(t => t.FechaCreacion <= fechaHasta.Value);

        return await query.CountAsync();
    }

    public async Task<IEnumerable<Ticket>> GetOverdueAsync(bool asNoTracking = true)
    {
        var query = _dbSet
            .Include(t => t.Prioridad)
            .Include(t => t.Estado)
            .Where(t => t.Estado.EsFinal == false);

        if (asNoTracking) query = query.AsNoTracking();

        return await query.ToListAsync();
    }
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(HelpDeskDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, bool asNoTracking = true)
    {
        var query = _dbSet.Where(u => u.Email == email);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(string role, bool asNoTracking = true)
    {
        var query = _dbSet.Where(u => u.Rol.ToString() == role);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetActiveTechniciansAsync(bool asNoTracking = true)
    {
        var query = _dbSet.Where(u => u.Rol == HelpDesk.Shared.Enums.UserRole.Tecnico && u.Activo);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<User>> GetFilteredAsync(
        string? role = null,
        bool? activo = null,
        string? search = null,
        int page = 1,
        int pageSize = 20,
        bool asNoTracking = true)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(role) && Enum.TryParse<HelpDesk.Shared.Enums.UserRole>(role, out var roleEnum))
            query = query.Where(u => u.Rol == roleEnum);
        if (activo.HasValue) query = query.Where(u => u.Activo == activo.Value);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(u => u.NombreCompleto.Contains(search) || u.Email.Contains(search));

        if (asNoTracking) query = query.AsNoTracking();

        return await query
            .OrderBy(u => u.NombreCompleto)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetFilteredCountAsync(
        string? role = null,
        bool? activo = null,
        string? search = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(role) && Enum.TryParse<HelpDesk.Shared.Enums.UserRole>(role, out var roleEnum))
            query = query.Where(u => u.Rol == roleEnum);
        if (activo.HasValue) query = query.Where(u => u.Activo == activo.Value);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(u => u.NombreCompleto.Contains(search) || u.Email.Contains(search));

        return await query.CountAsync();
    }
}

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(HelpDeskDbContext context) : base(context) { }

    public async Task<Category?> GetByNameAsync(string nombre, bool asNoTracking = true)
    {
        var query = _dbSet.Where(c => c.Nombre == nombre);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Category>> GetActiveAsync(bool asNoTracking = true)
    {
        var query = _dbSet.Where(c => c.Activo);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.OrderBy(c => c.Nombre).ToListAsync();
    }
}

public class PriorityRepository : Repository<Priority>, IPriorityRepository
{
    public PriorityRepository(HelpDeskDbContext context) : base(context) { }

    public async Task<Priority?> GetByNameAsync(string nombre, bool asNoTracking = true)
    {
        var query = _dbSet.Where(p => p.Nombre == nombre);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Priority>> GetAllOrderedAsync(bool asNoTracking = true)
    {
        var query = _dbSet.OrderBy(p => p.Nivel);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync();
    }
}

public class StatusRepository : Repository<Status>, IStatusRepository
{
    public StatusRepository(HelpDeskDbContext context) : base(context) { }

    public async Task<Status?> GetByNameAsync(string nombre, bool asNoTracking = true)
    {
        var query = _dbSet.Where(s => s.Nombre == nombre);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Status>> GetAllOrderedAsync(bool asNoTracking = true)
    {
        var query = _dbSet.OrderBy(s => s.Orden);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync();
    }

    public async Task<Status?> GetInitialStatusAsync(bool asNoTracking = true)
    {
        var query = _dbSet.Where(s => s.Orden == 1);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync();
    }

    public async Task<Status?> GetClosedStatusAsync(bool asNoTracking = true)
    {
        var query = _dbSet.Where(s => s.EsFinal == true);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync();
    }
}

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(HelpDeskDbContext context) : base(context) { }

    public async Task<IEnumerable<Comment>> GetByTicketIdAsync(Guid ticketId, bool asNoTracking = true)
    {
        var query = _dbSet.Where(c => c.TicketId == ticketId).Include(c => c.Usuario);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.OrderBy(c => c.FechaCreacion).ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetPublicByTicketIdAsync(Guid ticketId, bool asNoTracking = true)
    {
        var query = _dbSet.Where(c => c.TicketId == ticketId && !c.EsInterno).Include(c => c.Usuario);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.OrderBy(c => c.FechaCreacion).ToListAsync();
    }
}

public class StatusHistoryRepository : Repository<StatusHistory>, IStatusHistoryRepository
{
    public StatusHistoryRepository(HelpDeskDbContext context) : base(context) { }

    public async Task<IEnumerable<StatusHistory>> GetByTicketIdAsync(Guid ticketId, bool asNoTracking = true)
    {
        var query = _dbSet.Where(h => h.TicketId == ticketId).Include(h => h.EstadoAnterior).Include(h => h.EstadoNuevo).Include(h => h.Usuario);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.OrderBy(h => h.FechaCambio).ToListAsync();
    }

    public async Task<StatusHistory> CreateAsync(StatusHistory entity)
    {
        entity.Id = Guid.NewGuid();
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}