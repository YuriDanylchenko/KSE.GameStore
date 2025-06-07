using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.Web.Mapping;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.ApplicationCore.Mapping;
using KSE.GameStore.DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using AutoMapper;

namespace KSE.GameStore.Tests.UnitTests.Services;

public class GameServiceTests
{
    private readonly Mock<IGameRepository> _mockGameRepo;
    private readonly Mock<IRepository<Genre, int>> _mockGenreRepo;
    private readonly Mock<IRepository<Platform, int>> _mockPlatformRepo;
    private readonly Mock<IRepository<Region, int>> _mockRegionRepo;
    private readonly Mock<IRepository<Publisher, int>> _mockPublisherRepo;
    private readonly Mock<ILogger<GameService>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly GameService _service;

    public GameServiceTests()
    {
        _mockGameRepo = new Mock<IGameRepository>();
        _mockGenreRepo = new Mock<IRepository<Genre, int>>();
        _mockPlatformRepo = new Mock<IRepository<Platform, int>>();
        _mockRegionRepo = new Mock<IRepository<Region, int>>();
        _mockPublisherRepo = new Mock<IRepository<Publisher, int>>();
        _mockLogger = new Mock<ILogger<GameService>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AddProfile<WebMappingProfile>();
            cfg.AddProfile<ApplicationCoreMappingProfile>();
        });
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

    public class GetGameById : GameServiceTests
    {
        [Fact]
        public async Task GetGameByIdAsync_ThrowsNotFound_WhenGameMissing()
        {
            // Arrange
            _mockGameRepo
                .Setup(r => r.GetGameByIdAsync(1))
                .ReturnsAsync((Game?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetGameByIdAsync(1));
        }

        [Fact]
        public async Task GetGameByIdAsync_ReturnsGame_WhenExists()
        {
            // Arrange
            var existingGameEntity = new Game
            {
                Id = 1,
                Title = "Test Game",
                Description = "Test Description",
                PublisherId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Publisher = new Publisher { Name = "Test Publisher" },
                Genres = new List<Genre> { new() { Name = "RPG" } },
                Platforms = new List<Platform> { new() { Name = "PC" } },
                Prices = new List<GamePrice>(),
                RegionPermissions = new List<Region> { new() { Code = "US", Name = "United States" } }
            };

            var currentPrice = new GamePrice
            {
                GameId = 1, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null,
                Game = existingGameEntity
            };
            var historicalPrice = new GamePrice
            {
                GameId = 1, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30), Game = existingGameEntity
            };

            existingGameEntity.Prices.Add(currentPrice);
            existingGameEntity.Prices.Add(historicalPrice);

            _mockGameRepo
                .Setup(r => r.GetGameByIdAsync(1))
                .ReturnsAsync(existingGameEntity);

            // Act
            var resultDto = await _service.GetGameByIdAsync(1);

            // Assert
            Assert.Equal(1, resultDto.Id);
            Assert.Equal("Test Game", resultDto.Title);
            Assert.Equal("Test Description", resultDto.Description);
            Assert.Equal("Test Publisher", resultDto.Publisher.Name);
            Assert.Single(resultDto.Genres);
            Assert.Equal("RPG", resultDto.Genres.First().Name);
            Assert.Single(resultDto.Platforms);
            Assert.Equal("PC", resultDto.Platforms.First().Name);
            Assert.NotNull(resultDto.Price);
            Assert.Equal(59.99m, resultDto.Price.Value);
            Assert.Equal(10, resultDto.Price.Stock);
            Assert.Single(resultDto.RegionPermissions!);
            Assert.Equal("US", resultDto.RegionPermissions!.First().Code);
            Assert.Equal("United States", resultDto.RegionPermissions!.First().Name);
        }
    }

    public class GetAllGames : GameServiceTests
    {
        [Fact]
        public async Task GetAllGamesAsync_ThrowsBadRequest_WhenPageNumberInvalid()
        {
            // Arrange
            int invalidPageNumber = 0;

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _service.GetAllGamesAsync(invalidPageNumber, 10));
        }

        [Fact]
        public async Task GetAllGamesAsync_ThrowsBadRequest_WhenPageSizeInvalid()
        {
            // Arrange
            int invalidPageSize = 0;

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _service.GetAllGamesAsync(1, invalidPageSize));
        }

        [Fact]
        public async Task GetAllGamesAsync_ReturnsMappedDTOs_WhenValidParametersProvided()
        {
            // Arrange
            var gameA = new Game
            {
                Id = 1,
                Title = "Some Game",
                Description = "Some Description",
                Publisher = new Publisher { Name = "Test Publisher" },
                Genres = new List<Genre> { new() { Id = 1, Name = "RPG" } },
                Platforms = new List<Platform> { new() { Id = 20, Name = "PC" } },
                Prices = new List<GamePrice>(),
                RegionPermissions = new List<Region> { new() { Code = "US", Name = "United States" } }
            };
            gameA.Prices.Add(new GamePrice
            {
                GameId = 1,
                Value = 59.99m,
                Stock = 10,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Game = gameA
            });

            var gameB = new Game
            {
                Id = 2,
                Title = "Another Game",
                Description = "Another Description",
                Publisher = new Publisher { Name = "Test Publisher" },
                Genres = new List<Genre> { new() { Id = 1, Name = "RPG" } },
                Platforms = new List<Platform> { new() { Id = 20, Name = "PC" } },
                Prices = new List<GamePrice>(),
                RegionPermissions = new List<Region> { new() { Code = "US", Name = "United States" } }
            };
            gameB.Prices.Add(new GamePrice
            {
                GameId = 2,
                Value = 49.99m,
                Stock = 5,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Game = gameB
            });

            var gameEntities = new List<Game> { gameA, gameB };

            _mockGameRepo
                .Setup(r => r.ListGamesAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(gameEntities);

            // Act
            var result = await _service.GetAllGamesAsync(1, 10);

            // Assert
            Assert.Equal(2, result.Count);

            // First game
            var dto1 = result[0];
            Assert.Equal(1, dto1.Id);
            Assert.Equal("Some Game", dto1.Title);
            Assert.Equal("Some Description", dto1.Description);
            Assert.Equal("Test Publisher", dto1.Publisher.Name);
            Assert.Single(dto1.Genres);
            Assert.Equal("RPG", dto1.Genres[0].Name);
            Assert.Single(dto1.Platforms);
            Assert.Equal("PC", dto1.Platforms[0].Name);
            Assert.NotNull(dto1.Price);
            Assert.Equal(59.99m, dto1.Price.Value);
            Assert.Equal(10, dto1.Price.Stock);
            Assert.Single(dto1.RegionPermissions!);
            Assert.Equal("US", dto1.RegionPermissions![0].Code);
            Assert.Equal("United States", dto1.RegionPermissions![0].Name);

            // Second game
            var dto2 = result[1];
            Assert.Equal(2, dto2.Id);
            Assert.Equal("Another Game", dto2.Title);
            Assert.Equal("Another Description", dto2.Description);
            Assert.Equal("Test Publisher", dto2.Publisher.Name);
            Assert.Single(dto2.Genres);
            Assert.Equal("RPG", dto2.Genres[0].Name);
            Assert.Single(dto2.Platforms);
            Assert.Equal("PC", dto2.Platforms[0].Name);
            Assert.NotNull(dto2.Price);
            Assert.Equal(49.99m, dto2.Price.Value);
            Assert.Equal(5, dto2.Price.Stock);
            Assert.Single(dto2.RegionPermissions!);
            Assert.Equal("US", dto2.RegionPermissions![0].Code);
            Assert.Equal("United States", dto2.RegionPermissions![0].Name);
        }
    }

    public class CreateGame : GenreServiceTests
    {
        [Fact]
        public async Task CreateGameAsync_ThrowsBadRequest_WhenTitleExists()
        {
            // Arrange
            var gameDto = new GameDTO
            {
                Title = "Test Game",
            };

            _mockGameRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateGameAsync(gameDto));
        }

        [Fact]
        public async Task CreateGameAsync_ThrowsNotFound_WhenPublisherMissing()
        {
            // Arrange
            var gameDto = new GameDTO
            {
                Publisher = new PublisherDTO { Id = 1, Name = "Created Publisher" }
            };

            _mockPublisherRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Publisher?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateGameAsync(gameDto));
        }

        [Fact]
        public async Task CreateGameAsync_ThrowsNotFound_WhenGenresMissing()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var gameDto = new GameDTO
            {
                Publisher = new PublisherDTO { Id = 1 },
                Genres = new List<GenreDTO> { new() { Id = 99 } }
            };

            _mockGameRepo
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
                .ReturnsAsync(false);

            _mockPublisherRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(publisher);

            _mockGenreRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Genre, bool>>>()))
                .ReturnsAsync(new List<Genre>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateGameAsync(gameDto));
        }

        [Fact]
        public async Task CreateGameAsync_ThrowsNotFound_WhenPlatformsMissing()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var genre = new Genre { Id = 1, Name = "Action" };
            var gameDto = new GameDTO
            {
                Publisher = new PublisherDTO { Id = 1 },
                Genres = new List<GenreDTO> { new() { Id = 1 } },
                Platforms = new List<PlatformDTO> { new() { Id = 99 } }
            };

            _mockGameRepo
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
                .ReturnsAsync(false);

            _mockPublisherRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(publisher);

            _mockGenreRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Genre, bool>>>()))
                .ReturnsAsync(new List<Genre> { genre });

            _mockPlatformRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Platform, bool>>>()))
                .ReturnsAsync(new List<Platform>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateGameAsync(gameDto));
        }

        [Fact]
        public async Task CreateGameAsync_ThrowsNotFound_WhenRegionsMissing()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var genre = new Genre { Id = 1, Name = "Action" };
            var platform = new Platform { Id = 1, Name = "PC" };
            var price = new GamePrice
                { Value = 10.0m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null, Game = null! };
            var gameDto = new GameDTO
            {
                Publisher = new PublisherDTO { Id = 1 },
                Genres = new List<GenreDTO> { new() { Id = 1 } },
                Platforms = new List<PlatformDTO> { new() { Id = 1 } },
                Price = new GamePriceDTO { Value = price.Value, Stock = price.Stock },
                RegionPermissions = new List<RegionDTO> { new() { Id = 99 } }
            };

            _mockGameRepo
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
                .ReturnsAsync(false);

            _mockPublisherRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(publisher);

            _mockGenreRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Genre, bool>>>()))
                .ReturnsAsync(new List<Genre> { genre });

            _mockPlatformRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Platform, bool>>>()))
                .ReturnsAsync(new List<Platform> { platform });

            _mockRegionRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Region, bool>>>()))
                .ReturnsAsync(new List<Region>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateGameAsync(gameDto));
        }

        [Fact]
        public async Task CreateGameAsync_CreatesGame_WhenRegionPermissionsNull()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var genres = new List<Genre> { new() { Id = 1, Name = "Action" } };
            var platforms = new List<Platform> { new() { Id = 1, Name = "PC" } };
            var price = new GamePrice
                { Value = 10.0m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null, Game = null! };

            var gameDto = new GameDTO
            {
                Title = "Created Game",
                Description = "Created Description",
                Publisher = new PublisherDTO { Id = 1 },
                Genres = new List<GenreDTO> { new() { Id = 1 } },
                Platforms = new List<PlatformDTO> { new() { Id = 1 } },
                Price = new GamePriceDTO { Value = price.Value, Stock = price.Stock },
                RegionPermissions = null
            };

            _mockGameRepo
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
                .ReturnsAsync(false);

            _mockPublisherRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(publisher);

            _mockGenreRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Genre, bool>>>()))
                .ReturnsAsync(genres);

            _mockPlatformRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Platform, bool>>>()))
                .ReturnsAsync(platforms);

            _mockRegionRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Region, bool>>>()))
                .ReturnsAsync(new List<Region>());

            Game captured = null!;
            var generatedId = 99;
            _mockGameRepo
                .Setup(r => r.AddAsync(It.IsAny<Game>()))
                .Callback<Game>(g =>
                {
                    g.Id = generatedId;
                    captured = g;
                })
                .Returns(Task.CompletedTask);

            _mockGameRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mockGameRepo
                .Setup(r => r.GetGameByIdAsync(generatedId))
                .ReturnsAsync(() => captured);

            // Act
            var result = await _service.CreateGameAsync(gameDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Created Game", result.Title);
            Assert.Equal("Created Description", result.Description);
            Assert.Equal(1, result.Publisher.Id);
            Assert.Equal("Test Publisher", result.Publisher.Name);
            Assert.Single(result.Genres);
            Assert.Equal("Action", result.Genres[0].Name);
            Assert.Single(result.Platforms);
            Assert.Equal("PC", result.Platforms[0].Name);
            Assert.NotNull(result.Price);
            Assert.Equal(10.0m, result.Price.Value);
            Assert.Equal(10, result.Price.Stock);
            Assert.Null(result.RegionPermissions);
        }

        [Fact]
        public async Task CreateGameAsync_CreatesGame_WhenEmptyRegionPermissions()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var genres = new List<Genre> { new() { Id = 1, Name = "Action" } };
            var platforms = new List<Platform> { new() { Id = 1, Name = "PC" } };
            var price = new GamePrice
                { Value = 10.0m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null, Game = null! };

            var gameDto = new GameDTO
            {
                Title = "Created Game",
                Description = "Created Description",
                Publisher = new PublisherDTO { Id = 1 },
                Genres = new List<GenreDTO> { new() { Id = 1 } },
                Platforms = new List<PlatformDTO> { new() { Id = 1 } },
                Price = new GamePriceDTO { Value = price.Value, Stock = price.Stock },
                RegionPermissions = new List<RegionDTO>()
            };

            _mockGameRepo
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
                .ReturnsAsync(false);

            _mockPublisherRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(publisher);

            _mockGenreRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Genre, bool>>>()))
                .ReturnsAsync(genres);

            _mockPlatformRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Platform, bool>>>()))
                .ReturnsAsync(platforms);

            _mockRegionRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Region, bool>>>()))
                .ReturnsAsync(new List<Region>());

            Game captured = null!;
            var generatedId = 99;
            _mockGameRepo
                .Setup(r => r.AddAsync(It.IsAny<Game>()))
                .Callback<Game>(g =>
                {
                    g.Id = generatedId;
                    captured = g;
                })
                .Returns(Task.CompletedTask);

            _mockGameRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mockGameRepo
                .Setup(r => r.GetGameByIdAsync(generatedId))
                .ReturnsAsync(() => captured);

            // Act
            var result = await _service.CreateGameAsync(gameDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Created Game", result.Title);
            Assert.Equal("Created Description", result.Description);
            Assert.Equal(1, result.Publisher.Id);
            Assert.Equal("Test Publisher", result.Publisher.Name);
            Assert.Single(result.Genres);
            Assert.Equal("Action", result.Genres[0].Name);
            Assert.Single(result.Platforms);
            Assert.Equal("PC", result.Platforms[0].Name);
            Assert.NotNull(result.Price);
            Assert.Equal(10.0m, result.Price.Value);
            Assert.Equal(10, result.Price.Stock);
            Assert.Empty(result.RegionPermissions!);
        }
    }

    public class UpdateGame : GameServiceTests
    {
        [Fact]
        public async Task UpdateGameAsync_ThrowsNotFound_WhenGameMissing()
        {
            // Arrange
            var gameDto = new GameDTO
            {
                Id = 99,
                Title = "Updated Game",
            };

            _mockGameRepo
                .Setup(r => r.GetGameWithCollectionsByIdAsync(99))
                .ReturnsAsync((Game?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateGameAsync(gameDto));
        }

        [Fact]
        public async Task UpdateGameAsync_ThrowsBadRequest_WhenTitleExists()
        {
            // Arrange
            var existingGameEntity = new Game
            {
                Id = 1,
                Title = "Test Game",
                Description = "Test Description",
                PublisherId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Publisher = new Publisher { Name = "Test Publisher" },
                Genres = new List<Genre> { new() { Name = "RPG" } },
                Platforms = new List<Platform> { new() { Name = "PC" } },
                Prices = new List<GamePrice>(),
                RegionPermissions = new List<Region> { new() { Code = "US", Name = "United States" } }
            };

            var currentPrice = new GamePrice
            {
                GameId = 1, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null,
                Game = existingGameEntity
            };
            var historicalPrice = new GamePrice
            {
                GameId = 1, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30), Game = existingGameEntity
            };

            existingGameEntity.Prices.Add(currentPrice);
            existingGameEntity.Prices.Add(historicalPrice);

            var gameDto = new GameDTO
            {
                Id = 1,
                Title = "Updated Game"
            };

            _mockGameRepo
                .Setup(r => r.GetGameWithCollectionsByIdAsync(gameDto.Id))
                .ReturnsAsync(existingGameEntity);

            _mockGameRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateGameAsync(gameDto));
        }

        [Fact]
        public async Task UpdateGameAsync_ThrowsNotFound_WhenPublisherMissing()
        {
            // Arrange
            var existingGameEntity = new Game
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
                GameId = 1, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null,
                Game = existingGameEntity
            };
            var historicalPrice = new GamePrice
            {
                GameId = 1, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30), Game = existingGameEntity
            };

            existingGameEntity.Prices.Add(currentPrice);
            existingGameEntity.Prices.Add(historicalPrice);

            var gameDto = new GameDTO
            {
                Id = 1,
                Title = "Updated Game",
                Publisher = new PublisherDTO { Id = 99 },
            };

            _mockGameRepo.Setup(r => r.GetGameWithCollectionsByIdAsync(1)).ReturnsAsync(existingGameEntity);

            _mockPublisherRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Publisher?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateGameAsync(gameDto));
        }

        [Fact]
        public async Task UpdateGameAsync_ThrowsNotFound_WhenGenresMissing()
        {
            // Arrange
            var existingGameEntity = new Game
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
                GameId = 1, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null,
                Game = existingGameEntity
            };
            var historicalPrice = new GamePrice
            {
                GameId = 1, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30), Game = existingGameEntity
            };

            existingGameEntity.Prices.Add(currentPrice);
            existingGameEntity.Prices.Add(historicalPrice);

            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var gameDto = new GameDTO
            {
                Id = 1,
                Publisher = new PublisherDTO { Id = 1, Name = "Created Publisher" },
                Genres = new List<GenreDTO> { new() { Id = 1, Name = "Action" } }
            };

            _mockGameRepo.Setup(r => r.GetGameWithCollectionsByIdAsync(1)).ReturnsAsync(existingGameEntity);

            _mockGameRepo
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
                .ReturnsAsync(false);

            _mockPublisherRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(publisher);

            _mockGenreRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Genre, bool>>>()))
                .ReturnsAsync(new List<Genre>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateGameAsync(gameDto));
        }

        [Fact]
        public async Task UpdateGameAsync_ThrowsNotFound_WhenPlatformsMissing()
        {
            // Arrange
            var existingGameEntity = new Game
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
                GameId = 1, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null,
                Game = existingGameEntity
            };
            var historicalPrice = new GamePrice
            {
                GameId = 1, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30), Game = existingGameEntity
            };

            existingGameEntity.Prices.Add(currentPrice);
            existingGameEntity.Prices.Add(historicalPrice);

            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var genre = new Genre { Id = 1, Name = "Action" };
            var gameDto = new GameDTO
            {
                Id = 1,
                Publisher = new PublisherDTO { Id = 1, Name = "Created Publisher" },
                Genres = new List<GenreDTO> { new() { Id = 1, Name = "Action" } },
                Platforms = new List<PlatformDTO> { new() { Id = 1, Name = "PC" } },
            };

            _mockGameRepo
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
                .ReturnsAsync(false);

            _mockPublisherRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(publisher);

            _mockGenreRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Genre, bool>>>()))
                .ReturnsAsync(new List<Genre> { genre });

            _mockPlatformRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Platform, bool>>>()))
                .ReturnsAsync(new List<Platform>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateGameAsync(gameDto));
        }

        [Fact]
        public async Task UpdateGameAsync_ThrowsNotFound_WhenRegionsMissing()
        {
            // Arrange
            var existingGameEntity = new Game
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
                GameId = 1, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null,
                Game = existingGameEntity
            };
            var historicalPrice = new GamePrice
            {
                GameId = 1, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30), Game = existingGameEntity
            };

            existingGameEntity.Prices.Add(currentPrice);
            existingGameEntity.Prices.Add(historicalPrice);

            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var genre = new Genre { Id = 1, Name = "Action" };
            var platform = new Platform { Id = 1, Name = "PC" };
            var gameDto = new GameDTO
            {
                Id = 1,
                Publisher = new PublisherDTO { Id = 1, Name = "Created Publisher" },
                Genres = new List<GenreDTO> { new() { Id = 1, Name = "Action" } },
                Platforms = new List<PlatformDTO> { new() { Id = 1, Name = "PC" } },
                Price = new GamePriceDTO { Value = 20.0m, Stock = 5 },
                RegionPermissions = new List<RegionDTO> { new() { Id = 1, Code = "US", Name = "United States" } }
            };

            _mockGameRepo
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
                .ReturnsAsync(false);

            _mockPublisherRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(publisher);

            _mockGenreRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Genre, bool>>>()))
                .ReturnsAsync(new List<Genre> { genre });

            _mockPlatformRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Platform, bool>>>()))
                .ReturnsAsync(new List<Platform> { platform });

            _mockRegionRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Region, bool>>>()))
                .ReturnsAsync(new List<Region>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateGameAsync(gameDto));
        }

        [Fact]
        public async Task UpdateGameAsync_CreatesGame_WhenEmptyRegionPermissions()
        {
            // Arrange
            var existingGameEntity = new Game
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
                GameId = 1, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null,
                Game = existingGameEntity
            };
            var historicalPrice = new GamePrice
            {
                GameId = 1, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30), Game = existingGameEntity
            };

            existingGameEntity.Prices.Add(currentPrice);
            existingGameEntity.Prices.Add(historicalPrice);

            var publisher = new Publisher { Id = 1, Name = "Updated Publisher" };
            var genres = new List<Genre>();
            var platforms = new List<Platform> { new() { Id = 1, Name = "Xbox" } };
            var price = new GamePrice
                { Value = 10.0m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null, Game = null! };

            var gameDto = new GameDTO
            {
                Id = 1,
                Title = "Updated Game",
                Description = "Updated Description",
                Publisher = new PublisherDTO { Id = 1 },
                Genres = new List<GenreDTO>(),
                Platforms = new List<PlatformDTO> { new() { Id = 1 } },
                Price = new GamePriceDTO { Value = price.Value, Stock = price.Stock },
                RegionPermissions = null
            };

            _mockGameRepo
                .Setup(r => r.GetGameWithCollectionsByIdAsync(1))
                .ReturnsAsync(existingGameEntity);

            _mockGameRepo
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
                .ReturnsAsync(false);

            _mockPublisherRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(publisher);

            _mockGenreRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Genre, bool>>>()))
                .ReturnsAsync(genres);

            _mockPlatformRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Platform, bool>>>()))
                .ReturnsAsync(platforms);

            _mockRegionRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Region, bool>>>()))
                .ReturnsAsync(new List<Region>());

            Game captured = null!;
            _mockGameRepo
                .Setup(r => r.Update(It.IsAny<Game>()))
                .Callback<Game>(g => captured = g);

            _mockGameRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mockGameRepo
                .Setup(r => r.GetGameByIdAsync(1))
                .ReturnsAsync(() => captured);

            // Act
            var result = await _service.UpdateGameAsync(gameDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Game", result.Title);
            Assert.Equal("Updated Description", result.Description);
            Assert.Equal(1, result.Publisher.Id);
            Assert.Equal("Updated Publisher", result.Publisher.Name);
            Assert.Empty(result.Genres);
            Assert.Single(result.Platforms);
            Assert.Equal("Xbox", result.Platforms[0].Name);
            Assert.NotNull(result.Price);
            Assert.Equal(10.0m, result.Price.Value);
            Assert.Equal(10, result.Price.Stock);
            Assert.Null(result.RegionPermissions);
        }

        [Fact]
        public async Task UpdateGameAsync_CreatesGame_WhenRegionPermissionsNull()
        {
            // Arrange
            var existingGameEntity = new Game
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
                GameId = 1, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null,
                Game = existingGameEntity
            };
            var historicalPrice = new GamePrice
            {
                GameId = 1, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30), Game = existingGameEntity
            };

            existingGameEntity.Prices.Add(currentPrice);
            existingGameEntity.Prices.Add(historicalPrice);

            var publisher = new Publisher { Id = 1, Name = "Updated Publisher" };
            var genres = new List<Genre>();
            var platforms = new List<Platform> { new() { Id = 1, Name = "Xbox" } };
            var price = new GamePrice
                { Value = 10.0m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null, Game = null! };

            var gameDto = new GameDTO
            {
                Id = 1,
                Title = "Updated Game",
                Description = "Updated Description",
                Publisher = new PublisherDTO { Id = 1 },
                Genres = new List<GenreDTO>(),
                Platforms = new List<PlatformDTO> { new() { Id = 1 } },
                Price = new GamePriceDTO { Value = price.Value, Stock = price.Stock },
                RegionPermissions = null
            };

            _mockGameRepo
                .Setup(r => r.GetGameWithCollectionsByIdAsync(1))
                .ReturnsAsync(existingGameEntity);

            _mockGameRepo
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Game, bool>>>()))
                .ReturnsAsync(false);

            _mockPublisherRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(publisher);

            _mockGenreRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Genre, bool>>>()))
                .ReturnsAsync(genres);

            _mockPlatformRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Platform, bool>>>()))
                .ReturnsAsync(platforms);

            _mockRegionRepo
                .Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Region, bool>>>()))
                .ReturnsAsync(new List<Region>());

            Game captured = null!;
            _mockGameRepo
                .Setup(r => r.Update(It.IsAny<Game>()))
                .Callback<Game>(g => captured = g);

            _mockGameRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mockGameRepo
                .Setup(r => r.GetGameByIdAsync(1))
                .ReturnsAsync(() => captured);

            // Act
            var result = await _service.UpdateGameAsync(gameDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Game", result.Title);
            Assert.Equal("Updated Description", result.Description);
            Assert.Equal(1, result.Publisher.Id);
            Assert.Equal("Updated Publisher", result.Publisher.Name);
            Assert.Empty(result.Genres);
            Assert.Single(result.Platforms);
            Assert.Equal("Xbox", result.Platforms[0].Name);
            Assert.NotNull(result.Price);
            Assert.Equal(10.0m, result.Price.Value);
            Assert.Equal(10, result.Price.Stock);
            Assert.Null(result.RegionPermissions);
        }
    }

    public class GetGamesByGenre : GameServiceTests
    {
        [Fact]
        public async Task GetGamesByGenreAsync_ThrowsNotFound_WhenGenreMissing()
        {
            // Arrange
            _mockGenreRepo
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Genre?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _service.GetGamesByGenreAsync(99));
        }

        [Fact]
        public async Task GetGamesByGenreAsync_ReturnsMappedDTOs_WhenGenreExists()
        {
            // Arrange
            var genre = new Genre { Id = 10, Name = "RPG", Games = new List<Game>() };

            var gameA = new Game
            {
                Id = 1,
                Title = "Some Game",
                Description = "Some Description",
                Publisher = new Publisher { Name = "Test Publisher" },
                Genres = new List<Genre> { genre },
                Platforms = new List<Platform> { new() { Id = 1, Name = "PC" } },
                Prices = new List<GamePrice>(),
                RegionPermissions = new List<Region> { new() { Code = "US", Name = "United States" } }
            };
            gameA.Prices.Add(new GamePrice
            {
                GameId = 1,
                Value = 59.99m,
                Stock = 10,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Game = gameA
            });

            var gameB = new Game
            {
                Id = 2,
                Title = "Another Game",
                Description = "Another Description",
                Publisher = new Publisher { Name = "Test Publisher" },
                Genres = new List<Genre> { genre },
                Platforms = new List<Platform> { new() { Id = 1, Name = "PC" } },
                Prices = new List<GamePrice>(),
                RegionPermissions = new List<Region> { new() { Code = "US", Name = "United States" } }
            };
            gameB.Prices.Add(new GamePrice
            {
                GameId = 2,
                Value = 49.99m,
                Stock = 5,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Game = gameB
            });

            genre.Games.Add(gameA);
            genre.Games.Add(gameB);

            _mockGenreRepo
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(genre);

            _mockGameRepo
                .Setup(r => r.GetGamesByGenreAsync(10))
                .ReturnsAsync(new List<Game> { gameA, gameB });

            // Act
            var result = await _service.GetGamesByGenreAsync(10);

            // Assert
            Assert.Equal(2, result.Count);

            // First game
            var dtoA = result[0];
            Assert.Equal(1, dtoA.Id);
            Assert.Equal("Some Game", dtoA.Title);
            Assert.Equal("Some Description", dtoA.Description);
            Assert.Equal("Test Publisher", dtoA.Publisher.Name);
            Assert.Single(dtoA.Genres);
            Assert.Equal("RPG", dtoA.Genres[0].Name);
            Assert.Single(dtoA.Platforms);
            Assert.Equal("PC", dtoA.Platforms[0].Name);
            Assert.NotNull(dtoA.Price);
            Assert.Equal(59.99m, dtoA.Price.Value);
            Assert.Equal(10, dtoA.Price.Stock);
            Assert.Single(dtoA.RegionPermissions!);
            Assert.Equal("US", dtoA.RegionPermissions![0].Code);
            Assert.Equal("United States", dtoA.RegionPermissions![0].Name);

            // Second game
            var dtoB = result[1];
            Assert.Equal(2, dtoB.Id);
            Assert.Equal("Another Game", dtoB.Title);
            Assert.Equal("Another Description", dtoB.Description);
            Assert.Equal("Test Publisher", dtoB.Publisher.Name);
            Assert.Single(dtoB.Genres);
            Assert.Equal("RPG", dtoB.Genres[0].Name);
            Assert.Single(dtoB.Platforms);
            Assert.Equal("PC", dtoB.Platforms[0].Name);
            Assert.NotNull(dtoB.Price);
            Assert.Equal(49.99m, dtoB.Price.Value);
            Assert.Equal(5, dtoB.Price.Stock);
            Assert.Single(dtoB.RegionPermissions!);
            Assert.Equal("US", dtoB.RegionPermissions![0].Code);
            Assert.Equal("United States", dtoB.RegionPermissions![0].Name);
        }
    }

    public class GetGamesByPlatform : GameServiceTests
    {
        [Fact]
        public async Task GetGamesByPlatformAsync_ThrowsNotFound_WhenPlatformMissing()
        {
            // Arrange
            _mockPlatformRepo
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Platform?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _service.GetGamesByPlatformAsync(99));
        }

        [Fact]
        public async Task GetGamesByPlatformAsync_ReturnsMappedDTOs_WhenPlatformExists()
        {
            // Arrange
            var platform = new Platform { Id = 20, Name = "PC", Games = new List<Game>() };

            var gameA = new Game
            {
                Id = 1,
                Title = "Some Game",
                Description = "Some Description",
                Publisher = new Publisher { Name = "Test Publisher" },
                Genres = new List<Genre> { new() { Id = 1, Name = "RPG" } },
                Platforms = new List<Platform> { platform },
                Prices = new List<GamePrice>(),
                RegionPermissions = new List<Region> { new() { Code = "US", Name = "United States" } }
            };
            gameA.Prices.Add(new GamePrice
            {
                GameId = 1,
                Value = 59.99m,
                Stock = 10,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Game = gameA
            });

            var gameB = new Game
            {
                Id = 2,
                Title = "Another Game",
                Description = "Another Description",
                Publisher = new Publisher { Name = "Test Publisher" },
                Genres = new List<Genre> { new() { Id = 1, Name = "RPG" } },
                Platforms = new List<Platform> { platform },
                Prices = new List<GamePrice>(),
                RegionPermissions = new List<Region> { new() { Code = "US", Name = "United States" } }
            };
            gameB.Prices.Add(new GamePrice
            {
                GameId = 2,
                Value = 49.99m,
                Stock = 5,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Game = gameB
            });

            platform.Games.Add(gameA);
            platform.Games.Add(gameB);

            _mockPlatformRepo
                .Setup(r => r.GetByIdAsync(20))
                .ReturnsAsync(platform);

            _mockGameRepo
                .Setup(r => r.GetGamesByPlatformAsync(20))
                .ReturnsAsync(new List<Game> { gameA, gameB });

            // Act
            var result = await _service.GetGamesByPlatformAsync(20);

            // Assert
            Assert.Equal(2, result.Count);

            // First game
            var dtoA = result[0];
            Assert.Equal(1, dtoA.Id);
            Assert.Equal("Some Game", dtoA.Title);
            Assert.Equal("Some Description", dtoA.Description);
            Assert.Equal("Test Publisher", dtoA.Publisher.Name);
            Assert.Single(dtoA.Genres);
            Assert.Equal("RPG", dtoA.Genres[0].Name);
            Assert.Single(dtoA.Platforms);
            Assert.Equal("PC", dtoA.Platforms[0].Name);
            Assert.NotNull(dtoA.Price);
            Assert.Equal(59.99m, dtoA.Price.Value);
            Assert.Equal(10, dtoA.Price.Stock);
            Assert.Single(dtoA.RegionPermissions!);
            Assert.Equal("US", dtoA.RegionPermissions![0].Code);
            Assert.Equal("United States", dtoA.RegionPermissions![0].Name);

            // Second game
            var dtoB = result[1];
            Assert.Equal(2, dtoB.Id);
            Assert.Equal("Another Game", dtoB.Title);
            Assert.Equal("Another Description", dtoB.Description);
            Assert.Equal("Test Publisher", dtoB.Publisher.Name);
            Assert.Single(dtoB.Genres);
            Assert.Equal("RPG", dtoB.Genres[0].Name);
            Assert.Single(dtoB.Platforms);
            Assert.Equal("PC", dtoB.Platforms[0].Name);
            Assert.NotNull(dtoB.Price);
            Assert.Equal(49.99m, dtoB.Price.Value);
            Assert.Equal(5, dtoB.Price.Stock);
            Assert.Single(dtoB.RegionPermissions!);
            Assert.Equal("US", dtoB.RegionPermissions![0].Code);
            Assert.Equal("United States", dtoB.RegionPermissions![0].Name);
        }
    }
}