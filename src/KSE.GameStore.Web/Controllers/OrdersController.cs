using KSE.GameStore.ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] Guid? userId, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] OrderStatus? status)
    {
        var orders = await _orderService.GetOrdersAsync(userId, from, to, status);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }
}
