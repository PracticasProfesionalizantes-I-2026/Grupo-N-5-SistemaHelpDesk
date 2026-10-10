using HelpDesk.DataAccess.Data;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Enums;
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

    public async Task<IEnumerable<Ticket>> FilterAsync(TicketFilterDTO filter)
    {
        var query = _context.Tickets.AsQueryable();

        if (filter.PrioridadId.HasValue)
            query = query.Where(t => t.PrioridadId == filter.PrioridadId.Value);

        if (filter.EstadoId.HasValue)
            query = query.Where(t => t.EstadoId == filter.EstadoId.Value);

        if (filter.CategoriaId.HasValue)
            query = query.Where(t => t.CategoriaId == filter.CategoriaId.Value);

        if (filter.EmpleadoId.HasValue)
            query = query.Where(t => t.EmpleadoId == filter.EmpleadoId.Value);

        if (filter.TecnicoId.HasValue)
            query = query.Where(t => t.TecnicoId == filter.TecnicoId.Value);

        if (filter.FechaDesde.HasValue)
            query = query.Where(t => t.FechaCreacion >= filter.FechaDesde.Value);

        if (filter.FechaHasta.HasValue)
            query = query.Where(t => t.FechaCreacion <= filter.FechaHasta.Value);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByEmpleadoIdAsync(Guid empleadoId, bool asNoTracking = true)
    {
        var query = _dbSet.Where(t => t.EmpleadoId == empleadoId);
        query = query.Include(t => t.Prioridad);
        query = query.Include(t => t.Estado);
        query = query.Include(t => t.Categoria);
        query = query.Include(t => t.Empleado);
        query = query.Include(t => t.Tecnico);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.OrderByDescending(t => t.FechaCreacion).ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByTecnicoIdAsync(Guid tecnicoId, bool asNoTracking = true)
    {
        var query = _dbSet.Where(t => t.TecnicoId == tecnicoId);
        query = query.Include(t => t.Prioridad);
        query = query.Include(t => t.Estado);
        query = query.Include(t => t.Categoria);
        query = query.Include(t => t.Empleado);
        query = query.Include(t => t.Tecnico);
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
        var query = _dbSet.AsQueryable();
        query = query.Include(t => t.Prioridad);
        query = query.Include(t => t.Estado);
        query = query.Include(t => t.Categoria);
        query = query.Include(t => t.Empleado);
        query = query.Include(t => t.Tecnico);

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
        var query = _dbSet.Include(t => t.Prioridad).Include(t => t.Estado).Where(t => t.Estado.EsFinal == false);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.ToListAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket != null)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"No se encontró el ticket con Id {id}");
        }
    }

    public async Task UpdateStatusAsync(Guid ticketId, Guid estadoId)
    {
        var ticket = await _dbSet.FindAsync(ticketId);
        if (ticket != null)
        {
            ticket.EstadoId = estadoId;
            ticket.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
    public async Task<TicketStatisticsDto> GetStatisticsAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Tickets.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.FechaCreacion >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.FechaCreacion <= endDate.Value);

        var tickets = await query
            .Include(t => t.Estado)
            .Include(t => t.Tecnico)
            .ToListAsync();

        if (!tickets.Any())
            throw new InvalidOperationException("No hay datos suficientes para el periodo seleccionado.");

        int total = tickets.Count;
        int abiertos = tickets.Count(t => t.Estado.Nombre == "Abierto");
        int enProceso = tickets.Count(t => t.Estado.Nombre == "En Proceso");
        int resueltos = tickets.Count(t => t.Estado.Nombre == "Resuelto");
        int cerrados = tickets.Count(t => t.Estado.Nombre == "Cerrado");

        double promedioHoras = tickets
            .Where(t => t.FechaResolucion.HasValue)
            .Select(t => (t.FechaResolucion.Value - t.FechaCreacion).TotalHours)
            .DefaultIfEmpty(0)
            .Average();

        var ticketsPorTecnico = tickets
            .Where(t => t.Tecnico != null)
            .GroupBy(t => t.Tecnico!.NombreCompleto)
            .ToDictionary(g => g.Key, g => g.Count());

        double promedioSatisfaccion = await _context.Ratings
            .Where(r => tickets.Select(t => t.Id).Contains(r.TicketId))
            .Select(r => r.Puntuacion)
            .DefaultIfEmpty(0)
            .AverageAsync();

        return new TicketStatisticsDto(total, abiertos, enProceso, resueltos, cerrados, promedioHoras, ticketsPorTecnico, promedioSatisfaccion);
    }
    public async Task ResolveAsync(Guid ticketId, Guid usuarioId, bool cerrar = false)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Estado)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            throw new KeyNotFoundException("El ticket no existe.");

        if (ticket.Estado.Nombre != "En Proceso" && ticket.Estado.Nombre != "Abierto")
            throw new InvalidOperationException("El ticket no cumple las condiciones para ser resuelto.");

        // Buscar estado Resuelto o Cerrado
        var nuevoEstadoNombre = cerrar ? "Cerrado" : "Resuelto";
        var nuevoEstado = await _context.Estados.FirstOrDefaultAsync(s => s.Nombre == nuevoEstadoNombre);

        if (nuevoEstado == null)
            throw new InvalidOperationException($"No se encontró el estado '{nuevoEstadoNombre}' en la base de datos.");

        var estadoAnteriorId = ticket.Estado.Id;

        ticket.Estado = nuevoEstado;
        ticket.EstadoId = nuevoEstado.Id;
        ticket.FechaResolucion = DateTime.UtcNow;

        _context.Tickets.Update(ticket);

        // Auditoría con IDs
        _context.HistorialEstados.Add(new StatusHistory
        {
            Id = Guid.NewGuid(),
            TicketId = ticket.Id,
            EstadoAnteriorId = estadoAnteriorId,
            EstadoNuevoId = nuevoEstado.Id,
            UsuarioId = usuarioId,
            FechaCambio = DateTime.UtcNow,
            Observacion = cerrar ? "Ticket cerrado por supervisor" : "Ticket resuelto por supervisor"
        });

        await _context.SaveChangesAsync();
    }
    public async Task<Ticket?> GetUnassignedTicketAsync()
    {
        return await _context.Tickets
            .Include(t => t.Categoria)
            .FirstOrDefaultAsync(t => t.TecnicoId == null);
    }

    public async Task AssignTicketAsync(Guid ticketId, Guid tecnicoId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null)
            throw new KeyNotFoundException("El ticket no existe.");

        ticket.TecnicoId = tecnicoId;
        ticket.EstadoId = (await _context.Estados.FirstAsync(s => s.Nombre == "Asignado")).Id;
        ticket.FechaAsignacion = DateTime.UtcNow;

        _context.Tickets.Update(ticket);

        _context.HistorialEstados.Add(new StatusHistory
        {
            Id = Guid.NewGuid(),
            TicketId = ticket.Id,
            EstadoAnteriorId = null,
            EstadoNuevoId = ticket.EstadoId,
            UsuarioId = tecnicoId, // técnico asignado
            FechaCambio = DateTime.UtcNow,
            Observacion = "Asignación automática"
        });

        await _context.SaveChangesAsync();
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
        IOrderedQueryable<Priority> query = _dbSet.OrderBy(p => p.Nivel);
        if (asNoTracking) query = (IOrderedQueryable<Priority>)query.AsNoTracking();
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
        IOrderedQueryable<Status> query = _dbSet.OrderBy(s => s.Orden);
        if (asNoTracking) query = (IOrderedQueryable<Status>)query.AsNoTracking();
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
        var query = _dbSet.Where(c => c.TicketId == ticketId);
        query = query.Include(c => c.Usuario);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.OrderBy(c => c.FechaCreacion).ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetPublicByTicketIdAsync(Guid ticketId, bool asNoTracking = true)
    {
        var query = _dbSet.Where(c => c.TicketId == ticketId && !c.EsInterno);
        query = query.Include(c => c.Usuario);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.OrderBy(c => c.FechaCreacion).ToListAsync();
    }
}

public class StatusHistoryRepository : Repository<StatusHistory>, IStatusHistoryRepository
{
    public StatusHistoryRepository(HelpDeskDbContext context) : base(context) { }

    public async Task<IEnumerable<StatusHistory>> GetByTicketIdAsync(Guid ticketId, bool asNoTracking = true)
    {
        var query = _dbSet.Where(h => h.TicketId == ticketId);
        query = query.Include(h => h.EstadoAnterior);
        query = query.Include(h => h.EstadoNuevo);
        query = query.Include(h => h.Usuario);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.OrderBy(h => h.FechaCambio).ToListAsync();
    }

    public new async Task<StatusHistory> CreateAsync(StatusHistory entity)
    {
        entity.Id = Guid.NewGuid();
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}

public class TeamRepository : Repository<Team>, ITeamRepository
{
    public TeamRepository(HelpDeskDbContext context) : base(context) { }

    public async Task<Team?> GetByNameAsync(string nombre, bool asNoTracking = true)
    {
        var query = _dbSet.Where(t => t.Nombre == nombre);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Team>> GetActiveAsync(bool asNoTracking = true)
    {
        var query = _dbSet.Where(t => t.Activo);
        query = query.Include(t => t.Categoria);
        query = query.Include(t => t.Tecnicos);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.OrderBy(t => t.Nombre).ToListAsync();
    }

    public async Task<IEnumerable<Team>> GetByCategoryAsync(Guid categoriaId, bool asNoTracking = true)
    {
        var query = _dbSet.Where(t => t.CategoriaId == categoriaId);
        query = query.Include(t => t.Categoria);
        query = query.Include(t => t.Tecnicos);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.OrderBy(t => t.Nombre).ToListAsync();
    }

    public async Task<Team?> GetWithTecnicosAsync(Guid id, bool asNoTracking = true)
    {
        var query = _dbSet.Where(t => t.Id == id);
        query = query.Include(t => t.Categoria);
        query = query.Include(t => t.Tecnicos);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync();
    }

    public async Task AddTecnicoAsync(Guid teamId, Guid tecnicoId)
    {
        var team = await _dbSet.Include(t => t.Tecnicos).FirstOrDefaultAsync(t => t.Id == teamId);
        var tecnico = await _context.Usuarios.FindAsync(tecnicoId);
        if (team != null && tecnico != null && tecnico.Rol == HelpDesk.Shared.Enums.UserRole.Tecnico)
        {
            team.Tecnicos.Add(tecnico);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveTecnicoAsync(Guid teamId, Guid tecnicoId)
    {
        var team = await _dbSet.Include(t => t.Tecnicos).FirstOrDefaultAsync(t => t.Id == teamId);
        if (team != null)
        {
            var tecnico = team.Tecnicos.FirstOrDefault(t => t.Id == tecnicoId);
            if (tecnico != null)
            {
                team.Tecnicos.Remove(tecnico);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task<bool> HasTecnicoAsync(Guid teamId, Guid tecnicoId)
    {
        return await _dbSet.Where(t => t.Id == teamId).SelectMany(t => t.Tecnicos).AnyAsync(u => u.Id == tecnicoId);
    }

    public async Task<IEnumerable<Team>> GetFilteredAsync(
        Guid? categoriaId = null,
        bool? activo = null,
        string? search = null,
        int page = 1,
        int pageSize = 20,
        bool asNoTracking = true)
    {
        var query = _dbSet.AsQueryable();
        query = query.Include(t => t.Categoria);
        query = query.Include(t => t.Tecnicos);

        if (categoriaId.HasValue) query = query.Where(t => t.CategoriaId == categoriaId.Value);
        if (activo.HasValue) query = query.Where(t => t.Activo == activo.Value);
        if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.Nombre.Contains(search) || (t.Descripcion != null && t.Descripcion.Contains(search)));

        if (asNoTracking) query = query.AsNoTracking();

        return await query
            .OrderBy(t => t.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetFilteredCountAsync(
        Guid? categoriaId = null,
        bool? activo = null,
        string? search = null)
    {
        var query = _dbSet.AsQueryable();

        if (categoriaId.HasValue) query = query.Where(t => t.CategoriaId == categoriaId.Value);
        if (activo.HasValue) query = query.Where(t => t.Activo == activo.Value);
        if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.Nombre.Contains(search) || (t.Descripcion != null && t.Descripcion.Contains(search)));

        return await query.CountAsync();
    }
    public async Task<IEnumerable<User>> GetTechniciansByCategoryAsync(Guid categoriaId)
    {
        return await _context.Usuarios
            .Include(u => u.TicketsAsignados)
            .Where(u => u.Rol == UserRole.Tecnico && u.TicketsAsignados.Any(t => t.CategoriaId == categoriaId))
            .ToListAsync();
    }
}