using AutoMapper;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Requests.Games;
using KSE.GameStore.DataAccess.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable.Moq;
using System.Linq.Expressions;
using KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.Web.Mapping;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.ApplicationCore.Mapping;

namespace KSE.GameStore.Tests.UnitTests;

public class GameServiceTests
{
    private readonly Mock<IRepository<Game, int>> _mockGameRepo;
    private readonly Mock<IRepository<Genre, int>> _mockGenreRepo;
    private readonly Mock<IRepository<Platform, int>> _mockPlatformRepo;
    private readonly Mock<IRepository<Region, int>> _mockRegionRepo;
    private readonly Mock<IRepository<Publisher, int>> _mockPublisherRepo;
    private readonly Mock<ILogger<GameService>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly GameService _service;

    public GameServiceTests()
    {
        _mockGameRepo = new Mock<IRepository<Game, int>>();
        _mockGenreRepo = new Mock<IRepository<Genre, int>>();
        _mockPlatformRepo = new Mock<IRepository<Platform, int>>();
        _mockRegionRepo = new Mock<IRepository<Region, int>>();
        _mockPublisherRepo = new Mock<IRepository<Publisher, int>>();
        _mockLogger = new Mock<ILogger<GameService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _service = new GameService(
            _mockGameRepo.Object,
            _mockGenreRepo.Object,
            _mockPlatformRepo.Object,
            _mockRegionRepo.Object,
            _mockPublisherRepo.Object,
            _mockLogger.Object,
            _mapper);
    }

    [Fact]
    public async Task GetGameByIdAsync_ThrowsBadRequest_WhenIdInvalid()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => _service.GetGameByIdAsync(0));
    }

    [Fact]
    public async Task GetGameByIdAsync_ThrowsNotFound_WhenGameMissing()
    {
        _mockGameRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<Func<IQueryable<Game>, IQueryable<Game>>>()))
            .ReturnsAsync((Game)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetGameByIdAsync(1));
    }

    [Fact]
    public async Task GetGameByIdAsync_ReturnsGame_WhenExists()
    {
        var game = new Game
        {
            Id = 1,
            Title = "Test Game",
            Description = "Test Description",
            PublisherId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Publisher = new Publisher { Name = "Test Publisher" },
            Genres = new List<Genre> { new() { Id = 1, Name = "RPG" } },
            Platforms = new List<Platform> { new() { Id = 1, Name = "PC" } },
            Prices = new List<GamePrice>(),
            RegionPermissions = new List<Region> { new() { Code = "US", Name = "United States" } }
        };

        var currentPrice = new GamePrice
            { GameId = 1, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null, Game = game };
        var historicalPrice = new GamePrice
        {
            GameId = 1, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30), Game = game
        };

        game.Prices.Add(currentPrice);
        game.Prices.Add(historicalPrice);

        _mockGameRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<Func<IQueryable<Game>, IQueryable<Game>>>()))
            .ReturnsAsync(game);

        var result = await _service.GetGameByIdAsync(1);

        Assert.Equal(1, result.Id);
        Assert.Equal("Test Game", result.Title);
        Assert.Equal("Test Description", result.Description);
    }

    [Fact]
    public async Task CreateGameAsync_ThrowsBadRequest_WhenTitleExists()
    {
        var request = new CreateGameRequest("Existing", "...", 1, [1], [1],
            new CreateGamePriceRequest(10.0m, 10), null);

        _mockGameRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateGameAsync(request));
    }

    [Fact]
    public async Task CreateGameAsync_ThrowsNotFound_WhenPublisherMissing()
    {
        var request = new CreateGameRequest("New", "...", 99, [1], [1],
            new CreateGamePriceRequest(10, 10), null);

        _mockPublisherRepo.Setup(r => r.GetByIdAsync(99, null)).ReturnsAsync((Publisher)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateGameAsync(request));
    }

    [Fact]
    public async Task CreateGameAsync_CreatesGame_WhenValid()
    {
        var request = new CreateGameRequest("Test Game", "Test Description", 1, [1, 2], [1],
            new CreateGamePriceRequest(10.0m, 10), [1]);

        var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
        var genres = new List<Genre> { new() { Id = 1, Name = "Action" }, new() { Id = 2, Name = "Adventure" } };
        var platforms = new List<Platform> { new() { Id = 1, Name = "Xbox" } };
        var regions = new List<Region> { new() { Id = 1 } };

        var dto = new GameDTO { Id = 1 };

        _mockPublisherRepo.Setup(r => r.GetByIdAsync(1, null)).ReturnsAsync(publisher);
        _mockGenreRepo.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Genre, bool>>>(), 1, 10, null))
            .ReturnsAsync(genres);
        _mockPlatformRepo.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Platform, bool>>>(), 1, 10, null))
            .ReturnsAsync(platforms);
        _mockRegionRepo.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Region, bool>>>(), 1, 10, null))
            .ReturnsAsync(regions);
        _mockGameRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()));

        Game createdGame = null;
        _mockGameRepo.Setup(r => r.AddAsync(It.IsAny<Game>()))
            .Callback<Game>(g =>
            {
                createdGame = g;
                createdGame.Id = 1;
                createdGame.Title = request.Title;
                createdGame.Description = request.Description;
            })
            .Returns(Task.CompletedTask);

        IQueryable<Game> backingList = new List<Game>().AsQueryable();
        var mockQueryable = backingList.BuildMock();

        _mockGameRepo
            .Setup(r => r.Query())
            .Returns(() =>
            {
                if (createdGame is not null)
                    return new List<Game> { createdGame }.AsQueryable().BuildMock();
                return mockQueryable;
            });

        var result = await _service.CreateGameAsync(request);

        Assert.Equal(dto.Id, result.Id);
        _mockGameRepo.Verify(r => r.AddAsync(createdGame!), Times.Once);
        _mockGameRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateGameAsync_ThrowsNotFound_WhenGameMissing()
    {
        var request = new UpdateGameRequest(99, "New", "...", 1, [1], [1],
            new UpdateGamePriceRequest(10, 10), [1]);

        _mockGameRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<Func<IQueryable<Game>, IQueryable<Game>>>()))
            .ReturnsAsync((Game)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateGameAsync(request));
    }

    [Fact]
    public async Task UpdateGameAsync_UpdatesGame_WhenValid()
    {
        var request = new UpdateGameRequest(1, "Updated", "...", 1, [1], [1],
            new UpdateGamePriceRequest(20, 20), [1]);

        var existingGame = new Game
        {
            Id = 1,
            Title = "Test Game",
            Description = "Test Description",
            PublisherId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Publisher = new Publisher { Name = "Test Publisher" },
            Genres = new List<Genre> { new() { Id = 1, Name = "RPG" } },
            Platforms = new List<Platform> { new() { Id = 1, Name = "PC" } },
            Prices = new List<GamePrice>(),
            RegionPermissions = new List<Region> { new() { Code = "US", Name = "United States" } }
        };

        var currentPrice = new GamePrice
        {
            GameId = 1, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null, Game = existingGame
        };
        var historicalPrice = new GamePrice
        {
            GameId = 1, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30), Game = existingGame
        };

        existingGame.Prices.Add(currentPrice);
        existingGame.Prices.Add(historicalPrice);

        var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
        var genres = new List<Genre> { new() { Id = 1, Name = "Action" } };
        var platforms = new List<Platform> { new() { Id = 1, Name = "Xbox" } };
        var regions = new List<Region> { new() { Id = 1 } };

        var dto = new GameDTO { Id = 1 };

        _mockGameRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<Func<IQueryable<Game>, IQueryable<Game>>>()))
            .ReturnsAsync(existingGame);
        _mockPublisherRepo.Setup(r => r.GetByIdAsync(1, null)).ReturnsAsync(publisher);
        _mockGenreRepo.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Genre, bool>>>(), 1, 10, null))
            .ReturnsAsync(genres);
        _mockPlatformRepo.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Platform, bool>>>(), 1, 10, null))
            .ReturnsAsync(platforms);
        _mockRegionRepo.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Region, bool>>>(), 1, 10, null))
            .ReturnsAsync(regions);

        var backingList = new List<Game> { existingGame }.AsQueryable();
        var mockQueryable = backingList.BuildMock();

        _mockGameRepo
            .Setup(r => r.Query())
            .Returns(mockQueryable);

        var result = await _service.UpdateGameAsync(request);

        Assert.Equal(dto.Id, result.Id);
        Assert.NotNull(existingGame.Prices.First().EndDate); // Verify old price was ended
        Assert.Equal("Updated", existingGame.Title);
        Assert.Equal("...", existingGame.Description);
        _mockGameRepo.Verify(r => r.Update(existingGame), Times.Once);
        _mockGameRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_ThrowsNotFound_WhenGameMissing()
    {
        _mockGameRepo.Setup(r => r.GetByIdAsync(99, null)).ReturnsAsync((Game)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteGameAsync(99));
    }

    [Fact]
    public async Task DeleteGameAsync_DeletesGame_WhenExists()
    {
        var game = new Game
        {
            Id = 1,
            Title = "Test Game",
            Description = "Test Description",
            PublisherId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Publisher = new Publisher { Name = "Test Publisher" },
            Genres = new List<Genre> { new() { Id = 1, Name = "RPG" } },
            Platforms = new List<Platform> { new() { Id = 1, Name = "PC" } },
            Prices = new List<GamePrice>(),
            RegionPermissions = new List<Region> { new() { Code = "US", Name = "United States" } }
        };

        var currentPrice = new GamePrice
            { GameId = 1, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null, Game = game };
        var historicalPrice = new GamePrice
        {
            GameId = 1, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30), Game = game
        };

        game.Prices.Add(currentPrice);
        game.Prices.Add(historicalPrice);
        _mockGameRepo.Setup(r => r.GetByIdAsync(1, null)).ReturnsAsync(game);

        await _service.DeleteGameAsync(1);

        _mockGameRepo.Verify(r => r.Delete(game), Times.Once);
        _mockGameRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}