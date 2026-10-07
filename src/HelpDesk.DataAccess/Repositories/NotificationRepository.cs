using HelpDesk.DataAccess.Data;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.DataAccess.Repositories;

public class NotificationRepository
    : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(HelpDeskDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Notification>> GetByUsuarioIdAsync(
        Guid usuarioId,
        bool asNoTracking = true)
    {
        IQueryable<Notification> query = _dbSet
    .Where(n => n.UsuarioId == usuarioId)
    .OrderByDescending(n => n.FechaCreacion);

if (asNoTracking)
    query = query.AsNoTracking();

return await query.ToListAsync();
    }
}