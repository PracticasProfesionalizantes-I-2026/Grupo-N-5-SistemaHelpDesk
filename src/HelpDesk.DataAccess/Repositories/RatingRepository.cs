using HelpDesk.DataAccess.Data;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.DataAccess.Repositories;

public class RatingRepository : Repository<Rating>, IRatingRepository
{
    public RatingRepository(HelpDeskDbContext context) : base(context) { }

    public async Task<Rating?> GetByTicketIdAsync(Guid ticketId, bool asNoTracking = true)
    {
        var query = _dbSet.Where(r => r.TicketId == ticketId);
        query = query.Include(r => r.Usuario);
        if (asNoTracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync();
    }
}