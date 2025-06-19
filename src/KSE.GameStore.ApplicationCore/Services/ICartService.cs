using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.ApplicationCore.Services
{
    public interface ICartService
    {
        Task AddGameToCartAsync(Guid userId, int gameId, int quantity = 1);
        Task<IList<OrderItem>> GetGamesInCartAsync(Guid userId);
        Task RemoveGameFromCartAsync(Guid userId, int orderItemId);
        Task ClearCartAsync(Guid userId);
    }
}