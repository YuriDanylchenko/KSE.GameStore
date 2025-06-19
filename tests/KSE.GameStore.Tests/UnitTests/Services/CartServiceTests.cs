using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using Moq;
using System.Linq.Expressions;

namespace KSE.GameStore.Tests.UnitTests.Services;

public class CartServiceTests
{
    private readonly Mock<IRepository<Order, int>> _orderRepo = new();
    private readonly Mock<IRepository<User, Guid>> _userRepo = new();
    private readonly Mock<IGameRepository> _gameRepo = new();
    private readonly CartService _service;

    public CartServiceTests()
    {
        _service = new CartService(
            _orderRepo.Object,
            _userRepo.Object,
            _gameRepo.Object
        );
    }

    [Fact]
    public async Task AddGameToCartAsync_CreatesOrder_WhenNoneExists()
    {
        var userId = Guid.NewGuid();
        var gameId = 1;
        var user = new User { Id = userId, Name = "Test", Email = "test@test.com", Role = "User", RegionId = 1, Region = new Region { Id = 1, Name = "Test", Code = "T" } };
        var game = new Game { Id = gameId, Title = "Game", PublisherId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Publisher = new Publisher { Id = 1, Name = "Pub" }, Genres = new List<Genre>(), Platforms = new List<Platform>(), Prices = new List<GamePrice>() };

        _orderRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Order, bool>>>()))
            .ReturnsAsync([]);
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepo.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(game);

        await _service.AddGameToCartAsync(userId, gameId, 2);

        _orderRepo.Verify(r => r.AddAsync(It.Is<Order>(o => o.UserId == userId && o.OrderItems.Any(oi => oi.GameId == gameId && oi.Quantity == 2))), Times.Once);
    }

    [Fact]
    public async Task AddGameToCartAsync_IncrementsQuantity_WhenItemExists()
    {
        var userId = Guid.NewGuid();
        var gameId = 1;
        var order = new Order
        {
            Id = 1,
            UserId = userId,
            Status = OrderStatus.Initiated,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            User = new() { Id = userId, Name = "Test", Email = "test@test.com", Role = "User", RegionId = 1, Region = new Region { Id = 1, Name = "Test", Code = "T" } },
            OrderItems =
            [
                new() { Id = 1, GameId = gameId, Quantity = 1, OrderId = 1, Order = null!, Game = new Game { Id = gameId, Title = "Game", PublisherId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Publisher = new Publisher { Id = 1, Name = "Pub" }, Genres = new List<Genre>(), Platforms = new List<Platform>(), Prices = new List<GamePrice>() } }
            ]
        };
        order.OrderItems.First().Order = order;

        _orderRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Order, bool>>>()))
            .ReturnsAsync([order]);

        await _service.AddGameToCartAsync(userId, gameId, 3);

        Assert.Equal(4, order.OrderItems.First().Quantity);
        _orderRepo.Verify(r => r.Update(order), Times.Once);
    }

    [Fact]
    public async Task GetGamesInCartAsync_ReturnsOrderItems()
    {
        var userId = Guid.NewGuid();
        var order = new Order
        {
            Id = 1,
            UserId = userId,
            Status = OrderStatus.Initiated,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            User = new() { Id = userId, Name = "Test", Email = "test@test.com", Role = "User", RegionId = 1, Region = new Region { Id = 1, Name = "Test", Code = "T" } },
            OrderItems =
            [
                new() { Id = 1, GameId = 1, Quantity = 2, OrderId = 1, Order = null!, Game = new Game { Id = 1, Title = "Game", PublisherId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Publisher = new Publisher { Id = 1, Name = "Pub" }, Genres = new List<Genre>(), Platforms = new List<Platform>(), Prices = new List<GamePrice>() } }
            ]
        };
        order.OrderItems.First().Order = order;

        _orderRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Order, bool>>>()))
            .ReturnsAsync([order]);

        var result = await _service.GetGamesInCartAsync(userId);

        Assert.Single(result);
        Assert.Equal(2, result[0].Quantity);
    }

    [Fact]
    public async Task RemoveGameFromCartAsync_RemovesItem()
    {
        var userId = Guid.NewGuid();
        var orderItem = new OrderItem { Id = 1, GameId = 1, Quantity = 2, OrderId = 1, Order = null!, Game = new Game { Id = 1, Title = "Game", PublisherId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Publisher = new Publisher { Id = 1, Name = "Pub" }, Genres = new List<Genre>(), Platforms = new List<Platform>(), Prices = new List<GamePrice>() } };
        var order = new Order
        {
            Id = 1,
            UserId = userId,
            Status = OrderStatus.Initiated,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            User = new() { Id = userId, Name = "Test", Email = "test@test.com", Role = "User", RegionId = 1, Region = new Region { Id = 1, Name = "Test", Code = "T" } },
            OrderItems = [orderItem]
        };
        orderItem.Order = order;

        _orderRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Order, bool>>>()))
            .ReturnsAsync([order]);

        await _service.RemoveGameFromCartAsync(userId, orderItem.Id);

        Assert.Empty(order.OrderItems);
        _orderRepo.Verify(r => r.Update(order), Times.Once);
    }

    [Fact]
    public async Task ClearCartAsync_RemovesAllItems()
    {
        var userId = Guid.NewGuid();
        var order = new Order
        {
            Id = 1,
            UserId = userId,
            Status = OrderStatus.Initiated,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            User = new() { Id = userId, Name = "Test", Email = "test@test.com", Role = "User", RegionId = 1, Region = new Region { Id = 1, Name = "Test", Code = "T" } },
            OrderItems =
            [
                new() { Id = 1, GameId = 1, Quantity = 2, OrderId = 1, Order = null!, Game = new Game { Id = 1, Title = "Game", PublisherId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Publisher = new Publisher { Id = 1, Name = "Pub" }, Genres = new List<Genre>(), Platforms = new List<Platform>(), Prices = new List<GamePrice>() } }
            ]
        };
        order.OrderItems.First().Order = order;

        _orderRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Order, bool>>>()))
            .ReturnsAsync([order]);

        await _service.ClearCartAsync(userId);

        Assert.Empty(order.OrderItems);
        _orderRepo.Verify(r => r.Update(order), Times.Once);
    }
}