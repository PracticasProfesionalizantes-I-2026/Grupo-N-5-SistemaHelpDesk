using HelpDesk.DataAccess.Data;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.DataAccess.Repositories;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(HelpDeskDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Notification>> GetByUsuarioIdAsync(Guid usuarioId, bool asNoTracking = true)
    {
        IQueryable<Notification> query = _dbSet
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.FechaCreacion);

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    // ✅ Implements INotificationRepository.RegistrarAsync
    public async Task RegistrarAsync(Notification notification)
    {
        _dbSet.Add(notification);
        await _context.SaveChangesAsync();
    }

    // ✅ Implements INotificationRepository.MarcarComoLeidaAsync
    public async Task MarcarComoLeidaAsync(Guid notificationId)
    {
        var notif = await _dbSet.FindAsync(notificationId);
        if (notif is null) return;

        notif.Leida = true;
        await _context.SaveChangesAsync();
    }

    // ✅ Implements INotificationRepository.ActualizarEstadoAsync
    public async Task ActualizarEstadoAsync(Guid notificationId, bool entregada)
    {
        var notif = await _dbSet.FindAsync(notificationId);
        if (notif is null) return;

        notif.Entregada = entregada;
        await _context.SaveChangesAsync();
    }
}
