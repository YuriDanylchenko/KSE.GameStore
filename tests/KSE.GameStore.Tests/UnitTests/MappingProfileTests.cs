using AutoMapper;
using KSE.GameStore.ApplicationCore.Mapping;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Requests.Games;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.Web.Mapping;

namespace KSE.GameStore.Tests.UnitTests;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Configuration_ShouldBeValid()
    {
        // Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
    
    // ============ Game → GameDTO Mappings ============
    [Fact]
    public void Should_Map_Game_To_GameDto_Correctly()
    {
        // Arrange
        var game = new Game
        {
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

        var currentPrice = new GamePrice { GameId = 0, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null, Game = game };
        var historicalPrice = new GamePrice { GameId = 0, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(30), Game = game };
        
        game.Prices.Add(currentPrice);
        game.Prices.Add(historicalPrice);

        // Act
        var dto = _mapper.Map<GameDTO>(game);

        // Assert
        Assert.Equal(0, dto.Id);
        Assert.Equal("Test Game", dto.Title);
        Assert.Equal("Test Description", dto.Description);
        Assert.Equal("Test Publisher", dto.Publisher);
        
        var genre = Assert.Single(dto.Genres);
        Assert.Equal("RPG", genre.Name);
        
        var platform = Assert.Single(dto.Platforms);
        Assert.Equal("PC", platform.Name);
        
        Assert.NotNull(dto.Price);
        Assert.Equal(59.99m, dto.Price.Value);
        
        var region = Assert.Single(dto.RegionPermissions);
        Assert.Equal("US", region.Code);
    }
    
    [Fact]
    public void Should_Handle_Null_RegionPermissions()
    {
        var game = new Game
        {
            Title = "Test Game",
            Description = "Test Description",
            PublisherId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Publisher = new Publisher { Name = "Test Publisher" },
            Genres = new List<Genre> { new() { Id = 1, Name = "RPG" } },
            Platforms = new List<Platform> { new() { Id = 1, Name = "PC" } },
            Prices = new List<GamePrice>(),
            RegionPermissions = null
        };

        var currentPrice = new GamePrice { GameId = 0, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null, Game = game };
        var historicalPrice = new GamePrice { GameId = 0, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(30), Game = game };
        
        game.Prices.Add(currentPrice);
        game.Prices.Add(historicalPrice);
        
        var dto = _mapper.Map<GameDTO>(game);
        Assert.NotNull(dto.RegionPermissions);
        Assert.Empty(dto.RegionPermissions);
    }
    
    // ============ CreateGameRequest → Game Mappings ============
    [Fact]
    public void Should_Map_CreateRequest_To_Game_Correctly()
    {
        // Arrange
        var request = new CreateGameRequest(
            Title: "New Game",
            Description: "New Description",
            PublisherId: 1,
            GenreIds: new List<int> { 1, 2 },
            PlatformIds: new List<int> { 3 },
            new CreateGamePriceRequest(49.99m, 10),
            new List<int> { 4 });

        // Act
        var game = _mapper.Map<Game>(request);

        // Assert
        Assert.Equal(0, game.Id);
        Assert.Equal("New Game", game.Title);
        Assert.Equal("New Description", game.Description);
        Assert.Equal(1, game.PublisherId);
        Assert.Equal(DateTime.UtcNow.Date, game.CreatedAt.Date);
        Assert.Equal(DateTime.UtcNow.Date, game.UpdatedAt.Date);
        Assert.Null(game.Genres); 
        Assert.Null(game.Platforms); 
        Assert.Null(game.Prices); 
        Assert.Null(game.RegionPermissions);
    }
    
    [Fact]
    public void Should_Map_CreatePriceRequest_Correctly()
    {
        var request = new CreateGamePriceRequest(29.99m, 5);
        var price = _mapper.Map<GamePrice>(request);

        Assert.Equal(29.99m, price.Value);
        Assert.Equal(5, price.Stock);
        Assert.Equal(DateTime.UtcNow.Date, price.StartDate.Date);
        Assert.Null(price.EndDate);
        Assert.Equal(0, price.Id); 
    }

    // ============ UpdateGameRequest → Game Mappings ============
    [Fact]
    public void Should_Map_UpdateRequest_Without_Overwriting_CreatedAt()
    {
        // Arrange
        var game = new Game
        {
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

        var currentPrice = new GamePrice { GameId = 1, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null, Game = game };
        var historicalPrice = new GamePrice { GameId = 1, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(30), Game = game };
        
        game.Prices.Add(currentPrice);
        game.Prices.Add(historicalPrice);
        
        var updateRequest = new UpdateGameRequest(
            Id : game.Id,
            Title: "Updated Game",
            Description: "Updated Description",
            PublisherId: 2,
            GenreIds: new List<int> { 2, 3 },
            PlatformIds: new List<int> { 4 },
            Price: new UpdateGamePriceRequest(39.99m, 8),
            RegionPermissionIds: new List<int> { 5 });
        
        // Act
        _mapper.Map(updateRequest, game);
        
        // Assert
        Assert.Equal("Updated Game", game.Title);
        Assert.Equal("Updated Description", game.Description);
        Assert.Equal(2, game.PublisherId);
        Assert.Equal(DateTime.UtcNow.Date, game.UpdatedAt.Date);
        Assert.Equal(game.CreatedAt.Date, game.CreatedAt.Date); 
    }
    
    [Fact]
    public void Should_Map_UpdatePriceRequest_Correctly()
    {
        var request = new UpdateGamePriceRequest(39.99m, 8);
        var price = _mapper.Map<GamePrice>(request);

        Assert.Equal(39.99m, price.Value);
        Assert.Equal(8, price.Stock);
        Assert.Equal(DateTime.UtcNow.Date, price.StartDate.Date);
        Assert.Null(price.EndDate);
    }
    
    // ============ Publisher, Genre, Platform, GamePrice, Region Mappings ============
    [Fact]
    public void Map_Publisher_To_PublisherDTO_Should_Map_Correctly()
    {
        // Arrange
        var publisher = new Publisher { Name = "Test Publisher", WebsiteUrl = "https://test.com", Description = "Test Description" };

        // Act
        var dto = _mapper.Map<PublisherDTO>(publisher);

        // Assert
        Assert.Equal(0, dto.Id);
        Assert.Equal("Test Publisher", dto.Name);
        Assert.Equal("https://test.com", dto.WebsiteUrl);
        Assert.Equal("Test Description", dto.Description);
    }

    [Fact]
    public void Map_Publisher_With_Null_Optional_Fields_Should_Map_Correctly()
    {
        var publisher = new Publisher { Name = "Test", WebsiteUrl = null, Description = null };

        var dto = _mapper.Map<PublisherDTO>(publisher);

        Assert.Null(dto.WebsiteUrl);
        Assert.Null(dto.Description);
    }
    
    [Fact]
    public void Map_Genre_To_GenreDTO_Should_Map_Correctly()
    {
        var genre = new Genre { Name = "RPG" };
        var dto = _mapper.Map<GenreDTO>(genre);
    
        Assert.Equal(0, dto.Id);
        Assert.Equal("RPG", dto.Name);
    }

    [Fact]
    public void Map_Genre_With_Null_Games_Should_Not_Throw()
    {
        var genre = new Genre { Name = "RPG", Games = null };
        var dto = _mapper.Map<GenreDTO>(genre);
    
        Assert.NotNull(dto); 
    }
    
    [Fact]
    public void Map_Platform_To_PlatformDTO_Should_Map_Correctly()
    {
        var platform = new Platform { Name = "PC" };
        var dto = _mapper.Map<PlatformDTO>(platform);
    
        Assert.Equal(0, dto.Id);
        Assert.Equal("PC", dto.Name);
    }

    [Fact]
    public void Map_Platform_With_Null_Games_Should_Not_Throw()
    {
        var platform = new Platform { Name = "PC", Games = null };
        var dto = _mapper.Map<PlatformDTO>(platform);
    
        Assert.NotNull(dto); 
    }
    
    [Fact]
    public void Map_GamePrice_To_GamePriceDTO_Should_Map_Correctly()
    {
        var game = new Game
        {
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

        var currentPrice = new GamePrice { GameId = 0, Value = 59.99m, Stock = 10, StartDate = DateTime.UtcNow, EndDate = null, Game = game };
        var historicalPrice = new GamePrice { GameId = 0, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(30), Game = game };
        
        game.Prices.Add(currentPrice);
        game.Prices.Add(historicalPrice);
        
        var dto = _mapper.Map<GamePriceDTO>(currentPrice);
    
        Assert.Equal(0, dto.Id);
        Assert.Equal(59.99m, dto.Value);
        Assert.Equal(10, dto.Stock);
    }

    [Fact]
    public void Map_GamePrice_With_Null_Stock_Should_Map_Correctly()
    {
        var game = new Game
        {
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

        var currentPrice = new GamePrice { GameId = 0, Value = 59.99m, Stock = null, StartDate = DateTime.UtcNow, EndDate = null, Game = game };
        var historicalPrice = new GamePrice { GameId = 0, Value = 49.99m, Stock = 5, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(30), Game = game };
        
        game.Prices.Add(currentPrice);
        game.Prices.Add(historicalPrice);
        
        var dto = _mapper.Map<GamePriceDTO>(currentPrice);
    
        Assert.Null(dto.Stock);
    }

    [Fact]
    public void Map_Region_To_RegionDTO_Should_Map_Correctly()
    {
        var region = new Region { Code = "US", Name = "United States" };
    
        var dto = _mapper.Map<RegionDTO>(region);
    
        Assert.Equal(0, dto.Id);
        Assert.Equal("US", dto.Code);
        Assert.Equal("United States", dto.Name);
    }

    [Fact]
    public void Map_Region_With_Null_Games_Should_Map_Correctly()
    {
        var region = new Region { Code = "US", Name = "No Code", Games = null };
        var dto = _mapper.Map<RegionDTO>(region);
    
        Assert.NotNull(dto);
    }
}
