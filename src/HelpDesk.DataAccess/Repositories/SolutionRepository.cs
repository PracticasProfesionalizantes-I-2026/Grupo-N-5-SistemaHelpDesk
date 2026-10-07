using HelpDesk.DataAccess.Data;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.DataAccess.Repositories;

public class SolutionRepository : Repository<Solution>, ISolutionRepository
{
    public SolutionRepository(HelpDeskDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Solution>> GetByTicketIdAsync(
        Guid ticketId,
        bool asNoTracking = true)
    {
        IQueryable<Solution> query = _dbSet
            .Where(s => s.TicketId == ticketId)
            .Include(s => s.Tecnico);

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }
}
