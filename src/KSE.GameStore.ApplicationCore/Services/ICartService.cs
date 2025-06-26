using KSE.GameStore.ApplicationCore.Models.Output;

namespace KSE.GameStore.ApplicationCore.Services
{
    public interface ICartService
    {
        /// <summary>
        /// Adds a game to the user's cart.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="gameId">The ID of the game to add.</param>
        /// <param name="quantity">The quantity of the game to add. Defaults to 1.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddGameToCartAsync(Guid userId, int gameId, int quantity = 1);
        /// <summary>
        /// Retrieves the list of games in the user's cart.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task that represents the asynchronous operation, containing a list of CartItemDto.</returns>
        Task<List<CartItemDto>> GetGamesInCartAsync(Guid userId);
        /// <summary>
        /// Removes a game from the user's cart.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="orderItemId">The ID of the order item to remove.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveGameFromCartAsync(Guid userId, int orderItemId);
        /// <summary>
        /// Clears all items from the user's cart.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ClearCartAsync(Guid userId);
    }
}