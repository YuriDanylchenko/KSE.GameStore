using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.DataAccess.Repositories;

public interface IOrderRepository : IRepository<Order, int>
{
    Task<Order?> GetOrderWithCollectionsByIdAsync(int id);
    Task<Order?> GetOrderByUserId(Guid id);
}