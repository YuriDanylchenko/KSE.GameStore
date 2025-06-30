using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using Moq;
using System.Linq.Expressions;
using AutoMapper;
using KSE.GameStore.ApplicationCore.Mapping;
using KSE.GameStore.Web.Mapping;

namespace KSE.GameStore.Tests.UnitTests.Services;

public class CartServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepo = new();
    private readonly Mock<IRepository<User, Guid>> _userRepo = new();
    private readonly Mock<IGameRepository> _gameRepo = new();
    private readonly CartService _service;
    private readonly IMapper _mapper;

    public CartServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AddProfile<WebMappingProfile>();
            cfg.AddProfile<ApplicationCoreMappingProfile>();
        });

        _mapper = config.CreateMapper();

        _service = new CartService(
            _orderRepo.Object,
            _userRepo.Object,
            _gameRepo.Object,
            _mapper
        );
    }

    private static User NewUser(Guid id) => new()
    {
        Id = id,
        Name = "Test",
        Email = "test@test.com",
        RegionId = 1,
        Region = new Region { Id = 1, Name = "Test", Code = "T" }
    };

    private static Game NewGame(int id, decimal price)
    {
        var game = new Game
        {
            Id = id,
            Title = $"Game {id}",
            PublisherId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Publisher = new Publisher { Id = 1, Name = "Pub" },
            Genres = [],
            Platforms = [],
            Prices = []
        };
        game.Prices.Add(new GamePrice{ EndDate = null, StartDate = DateTime.UtcNow, Value = price, Stock = 100_000, Game = game, GameId = id});
        return game;
    }

    private static Order NewOrder(Guid userId, OrderItem orderItem) => new()
    {
        Id = 1,
        UserId = userId,
        Status = OrderStatus.Initiated,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        User = NewUser(userId),
        OrderItems = [orderItem]
    };

    private static OrderItem NewOrderItem(int gameId, Game game, int qty = 1) => new()
    {

        Id = 1,
        GameId = gameId,
        Quantity = qty,
        OrderId = 1,
        Order = null!,
        Game = game
    };

    [Fact]
    public async Task AddGameToCartAsync_CreatesOrder_WhenNoneExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameId = 1;
        var gamePrice = 1000;

        var user = NewUser(userId);
        var game = NewGame(gameId, gamePrice);

        _orderRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Order, bool>>>()))
            .ReturnsAsync([]);
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepo.Setup(r => r.GetGameByIdAsync(gameId)).ReturnsAsync(game);

        // Act
        await _service.AddGameToCartAsync(userId, gameId, 2);

        // Assert
        _orderRepo.Verify(r => r.AddAsync(It.Is<Order>(o => o.UserId == userId && o.OrderItems.Any(oi => oi.GameId == gameId && oi.Quantity == 2))), Times.Once);
    }

    [Fact]
    public async Task AddGameToCartAsync_IncrementsQuantity_WhenItemExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameId = 1;
        var gamePrice = 1000;
        var user = NewUser(userId);
        var game = NewGame(gameId, gamePrice);
        var item = NewOrderItem(game.Id, game, qty: 1);
        var order = NewOrder(userId, item);
        item.Order = order;

        _orderRepo.Setup(r => r.GetOrderByUserId(userId))
            .ReturnsAsync(order);
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepo.Setup(r => r.GetGameByIdAsync(gameId)).ReturnsAsync(game);

        // Act
        await _service.AddGameToCartAsync(userId, gameId, 3);

        // Assert
        Assert.Equal(4, order.OrderItems.First().Quantity);
        _orderRepo.Verify(r => r.Update(order), Times.Once);
    }

    [Fact]
    public async Task GetGamesInCartAsync_ReturnsOrderItems()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gamePrice = 1000;
        var game1 = NewGame(1, gamePrice);
        var game2 = NewGame(2, gamePrice);

        var user = NewUser(userId);
        var item1 = NewOrderItem(game1.Id, game1, qty: 2);
        var item2 = NewOrderItem(game2.Id, game2, qty: 1);
        var order = NewOrder(userId, item1);
        order.OrderItems.Add(item2);
        item1.Order = order;
        item2.Order = order;

        _orderRepo.Setup(r => r.GetOrderByUserId(userId))
            .ReturnsAsync(order);
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(game1);
        _gameRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(game2);

        // Act
        var result = await _service.GetGamesInCartAsync(userId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result.First(r => r.GameId == game1.Id).Quantity);
        Assert.Equal(1, result.First(r => r.GameId == game2.Id).Quantity);
    }

    [Fact]
    public async Task RemoveGameFromCartAsync_RemovesItem()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gameId = 1;
        var gamePrice = 1000;
        var user = NewUser(userId);
        var game = NewGame(gameId, gamePrice);
        var item = NewOrderItem(game.Id, game, qty: 1);
        var order = NewOrder(userId, item);
        item.Order = order;

        _orderRepo.Setup(r => r.GetOrderByUserId(userId))
            .ReturnsAsync(order);
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepo.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(game);

        // Act
        await _service.RemoveGameFromCartAsync(userId, item.Id);

        // Assert
        Assert.Empty(order.OrderItems);
        _orderRepo.Verify(r => r.Update(order), Times.Once);
    }

    [Fact]
    public async Task ClearCartAsync_RemovesAllItems()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var gamePrice = 1000;
        var game1 = NewGame(1, gamePrice);
        var game2 = NewGame(2, gamePrice);

        var user = NewUser(userId);
        var item1 = NewOrderItem(game1.Id, game1, qty: 2);
        var item2 = NewOrderItem(game2.Id, game2, qty: 1);
        var order = NewOrder(userId, item1);
        order.OrderItems.Add(item2);
        item1.Order = order;
        item2.Order = order;

        _orderRepo.Setup(r => r.GetOrderByUserId(userId))
            .ReturnsAsync(order);
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(game1);
        _gameRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(game2);

        // Act
        await _service.ClearCartAsync(userId);

        // Assert
        Assert.Empty(order.OrderItems);
        _orderRepo.Verify(r => r.Update(order), Times.Once);
    }
}