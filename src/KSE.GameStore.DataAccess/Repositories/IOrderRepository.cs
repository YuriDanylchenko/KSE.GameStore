using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.DataAccess.Repositories;

public interface IOrderRepository : IRepository<Order, int>
{
    Task<Order?> GetOrderWithCollectionsByIdAsync(int id);
    Task<Order?> GetOrderByUserId(Guid id);
    public Task<List<Order>> GetOrdersWithDetailsAsync(
        Guid? userId = null,
        DateTime? from = null,
        DateTime? to = null,
        OrderStatus? status = null);
}