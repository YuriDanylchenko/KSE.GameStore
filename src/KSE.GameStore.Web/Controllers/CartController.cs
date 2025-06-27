using KSE.GameStore.ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class CartController(ICartService cartService) : ControllerBase
{
    private readonly ICartService _cartService = cartService;

    [HttpPost("add")]
    public async Task<IActionResult> AddGameToCart([FromQuery] Guid userId, [FromQuery] int gameId, [FromQuery] int quantity = 1)
    {
        await _cartService.AddGameToCartAsync(userId, gameId, quantity);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetGamesInCart([FromQuery] Guid userId)
    {
        var items = await _cartService.GetGamesInCartAsync(userId);
        return Ok(items);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveGameFromCart([FromQuery] Guid userId, [FromQuery] int orderItemId)
    {
        await _cartService.RemoveGameFromCartAsync(userId, orderItemId);
        return Ok();
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart([FromQuery] Guid userId)
    {
        await _cartService.ClearCartAsync(userId);
        return Ok();
    }
}