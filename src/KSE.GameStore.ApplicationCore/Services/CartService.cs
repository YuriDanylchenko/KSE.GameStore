using AutoMapper;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;

namespace KSE.GameStore.ApplicationCore.Services
{
    public class CartService : ICartService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IMapper _mapper;

        public CartService(
            IOrderRepository orderRepository,
            IRepository<User, Guid> userRepository,
            IGameRepository gameRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
            _mapper = mapper;
        }

        public async Task AddGameToCartAsync(Guid userId, int gameId, int quantity = 1)
        {
            var order = await _orderRepository.GetOrderByUserId(userId);

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
            var game = await _gameRepository.GetGameByIdAsync(gameId)
                       ?? throw new InvalidOperationException("Game not found.");
            var currentPrice = await FindCurrentGamePrice(game);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                order.UpdatedAt = DateTime.UtcNow;
                existingItem.Price = existingItem.Quantity * currentPrice;
            }
            else
            {
                var newItem = new OrderItem
                {
                    GameId = game.Id,
                    OrderId = order.Id,
                    Quantity = quantity,
                    Order = order,
                    Game = game,
                    Price = currentPrice * quantity
                };
                order.OrderItems.Add(newItem);
                order.UpdatedAt = DateTime.UtcNow;
            }

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task<List<CartItemDto>> GetGamesInCartAsync(Guid userId)
        {
            var order = await _orderRepository.GetOrderByUserId(userId);

            return order is null
                ? []
                : _mapper.Map<List<CartItemDto>>(order.OrderItems);
        }

        public async Task RemoveGameFromCartAsync(Guid userId, int orderItemId)
        {
            var order = await _orderRepository.GetOrderByUserId(userId);

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
            var order = await _orderRepository.GetOrderByUserId(userId);

            if (order == null) return;

            order.OrderItems.Clear();
            order.UpdatedAt = DateTime.UtcNow;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
        }

        private Task<decimal> FindCurrentGamePrice(Game game)
        {
            if (game.Prices == null || game.Prices.Count == 0)
                throw new InvalidOperationException("No prices found for the game.");

            // Find the current price (EndDate == null)
            var currentPrice = game.Prices
                .FirstOrDefault(p => p.EndDate == null);

            if (currentPrice == null)
                throw new InvalidOperationException("Current price not found (missing EndDate == null).");

            return Task.FromResult(currentPrice.Value);
        }
    }
}