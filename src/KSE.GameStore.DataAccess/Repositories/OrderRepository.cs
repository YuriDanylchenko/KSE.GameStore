using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace KSE.GameStore.DataAccess.Repositories;

public class OrderRepository(GameStoreDbContext context) : Repository<Order, int>(context), IOrderRepository
{
    public async Task<Order?> GetOrderWithCollectionsByIdAsync(int id)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Game)
            .Include(o => o.User)
            .SingleOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetOrderByUserId(Guid id)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Game)
            .Include(o => o.User)
            .Where(o => o.UserId == id && o.Status == OrderStatus.Initiated)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Order>> GetOrdersWithDetailsAsync(Guid? userId = null, DateTime? from = null, DateTime? to = null,
        OrderStatus? status = null)
    {
        IQueryable<Order> query = _dbSet
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Game)
            .Include(o => o.User);

        if (userId.HasValue)
            query = query.Where(o => o.UserId == userId.Value);

        if (from.HasValue)
            query = query.Where(o => o.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(o => o.CreatedAt <= to.Value);

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        return await query.ToListAsync();
    }
}