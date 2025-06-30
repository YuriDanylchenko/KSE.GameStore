using AutoMapper;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;

namespace KSE.GameStore.ApplicationCore.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<List<OrderDTO>> GetOrdersAsync(Guid? userId = null, DateTime? from = null, DateTime? to = null, OrderStatus? status = null)
    {
        var orders = await _orderRepository.GetOrdersWithDetailsAsync(userId, from, to, status);
        return orders.Select(o => _mapper.Map<OrderDTO>(o)).ToList();
    }

    public async Task<OrderDTO?> GetOrderByIdAsync(int orderId)
    {
        var order = await _orderRepository.GetOrderWithCollectionsByIdAsync(orderId);
        return order == null ? null : _mapper.Map<OrderDTO>(order);
    }
}