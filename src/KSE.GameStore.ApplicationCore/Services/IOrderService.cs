using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.ApplicationCore.Services;

public interface IOrderService
{
    Task<List<OrderDTO>> GetOrdersAsync(Guid? userId = null, DateTime? from = null, DateTime? to = null, OrderStatus? status = null);
    Task<OrderDTO?> GetOrderByIdAsync(int orderId);
}