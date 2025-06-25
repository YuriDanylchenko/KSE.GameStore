using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.ApplicationCore.Services
{
    public interface ICartService
    {
        Task AddGameToCartAsync(Guid userId, int gameId, int quantity = 1);
        Task<List<CartItemDto>> GetGamesInCartAsync(Guid userId);
        Task RemoveGameFromCartAsync(Guid userId, int orderItemId);
        Task ClearCartAsync(Guid userId);
    }
}