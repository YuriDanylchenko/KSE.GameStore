using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;

namespace KSE.GameStore.ApplicationCore.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IGameRepository _gameRepository;

        public CartService(
            IRepository<Order, int> orderRepository,
            IRepository<User, Guid> userRepository,
            IGameRepository gameRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
        }

        public async Task AddGameToCartAsync(Guid userId, int gameId, int quantity = 1)
        {
            var orders = await _orderRepository.ListAllAsync(
                o => o.UserId == userId && o.Status == OrderStatus.Initiated);
            var order = orders.FirstOrDefault();

            if (order == null)
            {
                order = new Order
                {
                    UserId = userId,
                    Status = OrderStatus.Initiated,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    OrderItems = [],
                    User = await _userRepository.GetByIdAsync(userId)
                        ?? throw new InvalidOperationException("User not found.")
                };
                await _orderRepository.AddAsync(order);
                await _orderRepository.SaveChangesAsync();
            }

            var existingItem = order.OrderItems.FirstOrDefault(oi => oi.GameId == gameId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                order.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var game = await _gameRepository.GetByIdAsync(gameId)
                    ?? throw new InvalidOperationException("Game not found.");
                var newItem = new OrderItem
                {
                    GameId = game.Id,
                    OrderId = order.Id,
                    Quantity = quantity,
                    Order = order,
                    Game = game
                };
                order.OrderItems.Add(newItem);
                order.UpdatedAt = DateTime.UtcNow;
            }

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task<IList<OrderItem>> GetGamesInCartAsync(Guid userId)
        {
            var orders = await _orderRepository.ListAllAsync(
                o => o.UserId == userId && o.Status == OrderStatus.Initiated);
            var order = orders.FirstOrDefault();

            return order?.OrderItems.ToList() ?? [];
        }

        public async Task RemoveGameFromCartAsync(Guid userId, int orderItemId)
        {
            var orders = await _orderRepository.ListAllAsync(
                o => o.UserId == userId && o.Status == OrderStatus.Initiated);
            var order = orders.FirstOrDefault();

            if (order == null) return;

            var item = order.OrderItems.FirstOrDefault(oi => oi.Id == orderItemId);
            if (item != null)
            {
                order.OrderItems.Remove(item);
                order.UpdatedAt = DateTime.UtcNow;
                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var orders = await _orderRepository.ListAllAsync(
                o => o.UserId == userId && o.Status == OrderStatus.Initiated);
            var order = orders.FirstOrDefault();

            if (order == null) return;

            order.OrderItems.Clear();
            order.UpdatedAt = DateTime.UtcNow;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
        }
    }
}