using AutoMapper;
using KSE.GameStore.ApplicationCore.Mapping;
using KSE.GameStore.ApplicationCore.Models.Input;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.Tests.UnitTests.Mappings;

public class ApplicationCoreMappingProfileTests
{
    private readonly IMapper _mapper;

    public ApplicationCoreMappingProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AddProfile<ApplicationCoreMappingProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Configuration_IsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    public class ReadMappings : ApplicationCoreMappingProfileTests
    {
        [Fact]
        public void Maps_Game_To_GameDTO_With_Current_Price()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var genre = new Genre { Id = 1, Name = "RPG" };
            var platform = new Platform { Id = 1, Name = "PC" };
            var region = new Region { Id = 1, Name = "NA" };
            var price = new GamePrice
            {
                Id = 1,
                Value = 49.99m,
                Stock = 1,
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = null,
                Game = null!
            };


            var game = new Game
            {
                Id = 1,
                Title = "Test Game",
                Description = "Test Description",
                PublisherId = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Publisher = publisher,
                Genres = new List<Genre> { genre },
                Platforms = new List<Platform> { platform },
                Prices = new List<GamePrice>(),
                RegionPermissions = new List<Region> { region }
            };

            game.Prices.Add(price);

            // Act
            var dto = _mapper.Map<GameDTO>(game);

            // Assert
            Assert.Equal(game.Id, dto.Id);
            Assert.Equal(game.Title, dto.Title);
            Assert.Equal(game.Description, dto.Description);
            Assert.Equal(game.Publisher.Id, dto.Publisher.Id);
            Assert.Equal(game.Publisher.Name, dto.Publisher.Name);
            Assert.Single(dto.Genres);
            Assert.Equal(genre.Id, dto.Genres[0].Id);
            Assert.Equal(genre.Name, dto.Genres[0].Name);
            Assert.Single(dto.Platforms);
            Assert.Equal(platform.Id, dto.Platforms[0].Id);
            Assert.Equal(platform.Name, dto.Platforms[0].Name);
            Assert.NotNull(dto.Price);
            Assert.Equal(49.99m, dto.Price.Value);
            Assert.Equal(1, dto.Price.Stock);
            Assert.Single(dto.RegionPermissions!);
            Assert.Equal(1, dto.RegionPermissions![0].Id);
            Assert.Equal(region.Name, dto.RegionPermissions[0].Name);
        }

        [Fact]
        public void Maps_Game_To_GameDTO_With_Null_Price_When_No_Current_Price()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var genre = new Genre { Id = 1, Name = "RPG" };
            var platform = new Platform { Id = 1, Name = "PC" };
            var region = new Region { Id = 1, Name = "NA" };
            var price = new GamePrice
            {
                Id = 1,
                Value = 49.99m,
                Stock = 1,
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = DateTime.UtcNow.AddDays(-1),
                Game = null!
            };


            var game = new Game
            {
                Id = 1,
                Title = "Test Game",
                Description = "Test Description",
                PublisherId = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Publisher = publisher,
                Genres = new List<Genre> { genre },
                Platforms = new List<Platform> { platform },
                Prices = new List<GamePrice>(),
                RegionPermissions = new List<Region> { region }
            };

            game.Prices.Add(price);

            // Act
            var dto = _mapper.Map<GameDTO>(game);

            // Assert
            Assert.Null(dto.Price);
        }

        [Fact]
        public void Maps_Publisher_To_PublisherDTO()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };

            // Act
            var dto = _mapper.Map<PublisherDTO>(publisher);

            // Assert
            Assert.Equal(publisher.Id, dto.Id);
            Assert.Equal(publisher.Name, dto.Name);
        }

        [Fact]
        public void Maps_Genre_To_GenreDTO()
        {
            // Arrange
            var genre = new Genre { Id = 1, Name = "RPG" };

            // Act
            var dto = _mapper.Map<GenreDTO>(genre);

            // Assert
            Assert.Equal(genre.Id, dto.Id);
            Assert.Equal(genre.Name, dto.Name);
        }

        [Fact]
        public void Maps_Platform_To_PlatformDTO()
        {
            // Arrange
            var platform = new Platform { Id = 1, Name = "PC" };

            // Act
            var dto = _mapper.Map<PlatformDTO>(platform);

            // Assert
            Assert.Equal(platform.Id, dto.Id);
            Assert.Equal(platform.Name, dto.Name);
        }

        [Fact]
        public void Maps_Region_To_RegionDTO()
        {
            // Arrange
            var region = new Region { Id = 1, Name = "NA" };

            // Act
            var dto = _mapper.Map<RegionDTO>(region);

            // Assert
            Assert.Equal(region.Id, dto.Id);
            Assert.Equal(region.Name, dto.Name);
        }

        [Fact]
        public void Maps_GamePrice_To_GamePriceDTO()
        {
            // Arrange
            var price = new GamePrice
            {
                Id = 1,
                Value = 59.99m,
                Stock = 10,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = null,
                Game = null!
            };

            // Act
            var dto = _mapper.Map<GamePriceDTO>(price);

            // Assert
            Assert.Equal(price.Id, dto.Id);
            Assert.Equal(price.Value, dto.Value);
            Assert.Equal(price.Stock, dto.Stock);
        }
    }

    public class WriteMappings : ApplicationCoreMappingProfileTests
    {
        [Fact]
        public void Maps_CreateGameDTO_To_Game_With_Basic_Properties()
        {
            // Arrange
            var createDto = new CreateGameDTO(
                Title: "Test Game",
                Description: "Test Description",
                PublisherId: 1,
                GenreIds: new List<int> { 1, 2 },
                PlatformIds: new List<int> { 3 },
                PriceDto: new CreateGamePriceDTO(Value: 59.99m, Stock: 10),
                RegionPermissionIds: new List<int> { 4, 5 }
            );

            // Act
            var game = _mapper.Map<Game>(createDto);

            // Assert
            Assert.Equal(0, game.Id);
            Assert.Equal(createDto.Title, game.Title);
            Assert.Equal(createDto.Description, game.Description);
            Assert.Equal(createDto.PublisherId, game.PublisherId);
            Assert.True(game.CreatedAt > DateTime.MinValue);
            Assert.True((DateTime.UtcNow - game.UpdatedAt).TotalSeconds < 1);
            Assert.Empty(game.Genres);
            Assert.Empty(game.Platforms);
            Assert.Empty(game.Prices);
            Assert.Null(game.RegionPermissions);
        }

        [Fact]
        public void Maps_CreateGameDTO_To_Game_Ignores_Publisher_Genres_Platforms_Prices_RegionPermissions()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var genre = new Genre { Id = 1, Name = "RPG" };
            var platform = new Platform { Id = 1, Name = "PC" };
            var region = new Region { Id = 1, Name = "NA" };
            var price = new GamePrice
            {
                Id = 1,
                Value = 49.99m,
                Stock = 1,
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = null,
                Game = null!
            };

            var existingGame = new Game
            {
                Id = 1,
                Title = "Test Game",
                Description = "Test Description",
                PublisherId = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.Now,
                Publisher = publisher,
                Genres = new List<Genre> { genre },
                Platforms = new List<Platform> { platform },
                Prices = new List<GamePrice> { price },
                RegionPermissions = new List<Region> { region }
            };

            var createDto = new CreateGameDTO(
                Title: "Updated Game",
                PublisherId: 1,
                Description: "Updated Description",
                GenreIds: new List<int> { 2 },
                PlatformIds: new List<int> { 2 },
                PriceDto: new CreateGamePriceDTO(Value: 59.99m, Stock: 5),
                RegionPermissionIds: new List<int> { 2 }
            );

            // Act
            _mapper.Map(createDto, existingGame);

            // Assert
            Assert.Equal(1, existingGame.Id);
            Assert.Equal(1, existingGame.PublisherId);
            Assert.Equal(publisher, existingGame.Publisher);
            Assert.Single(existingGame.Genres);
            Assert.Equal(genre, existingGame.Genres.First());
            Assert.Single(existingGame.Platforms);
            Assert.Equal(platform, existingGame.Platforms.First());
            Assert.Single(existingGame.Prices);
            Assert.Equal(price, existingGame.Prices.First());
            Assert.Single(existingGame.RegionPermissions);
            Assert.Equal(region, existingGame.RegionPermissions.First());
        }

        [Fact]
        public void Maps_CreateGamePriceDTO_To_GamePrice()
        {
            // Arrange
            var createDto = new CreateGamePriceDTO(
                Value: 59.99m,
                Stock: 10
            );

            // Act
            var price = _mapper.Map<GamePrice>(createDto);

            // Assert
            Assert.Equal(0, price.Id);
            Assert.Equal(createDto.Value, price.Value);
            Assert.Equal(createDto.Stock, price.Stock);
            Assert.True(price.StartDate > DateTime.MinValue);
            Assert.Null(price.EndDate);
        }

        [Fact]
        public void Maps_UpdateGameDTO_To_Game_With_Basic_Properties()
        {
            // Arrange
            var updateDto = new UpdateGameDTO(
                Id: 1,
                Title: "Test Game",
                Description: "Test Description",
                PublisherId: 1,
                GenreIds: new List<int> { 1, 2 },
                PlatformIds: new List<int> { 3 },
                PriceDto: new UpdateGamePriceDTO(Value: 59.99m, Stock: 10),
                RegionPermissionIds: new List<int> { 4, 5 }
            );

            // Act
            var game = _mapper.Map<Game>(updateDto);

            // Assert
            Assert.Equal(updateDto.Id, game.Id);
            Assert.Equal(updateDto.Title, game.Title);
            Assert.Equal(updateDto.Description, game.Description);
            Assert.Equal(updateDto.PublisherId, game.PublisherId);
            Assert.Equal(game.CreatedAt, DateTime.Parse("0001-01-01T00:00:00Z").ToUniversalTime());
            Assert.True((DateTime.UtcNow - game.UpdatedAt).TotalSeconds < 1);
            Assert.Empty(game.Genres);
            Assert.Empty(game.Platforms);
            Assert.Empty(game.Prices);
            Assert.Null(game.RegionPermissions);
        }

        [Fact]
        public void Maps_UpdateGameDTO_To_Game_Ignores_CreatedAt_Publisher_Genres_Platforms_Prices_RegionPermissions()
        {
            // Arrange
            var publisher1 = new Publisher { Id = 2, Name = "Original Publisher" };
            var genre1 = new Genre { Id = 1, Name = "RPG" };
            var platform1 = new Platform { Id = 1, Name = "PC" };
            var region1 = new Region { Id = 1, Name = "NA" };
            var price1 = new GamePrice
            {
                Id = 1,
                Value = 49.99m,
                Stock = 1,
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = null,
                Game = null!
            };
            var originalDate = DateTime.UtcNow.AddDays(-10);

            var publisher2 = new Publisher { Id = 1, Name = "Updated Publisher" };
            var genre2 = new Genre { Id = 2, Name = "Action" };
            var platform2 = new Platform { Id = 2, Name = "Xbox" };
            var region2 = new Region { Id = 2, Name = "EU" };

            var existingGame = new Game
            {
                Id = 1,
                Title = "Test Game",
                Description = "Test Description",
                PublisherId = 2,
                CreatedAt = originalDate,
                UpdatedAt = DateTime.Now,
                Publisher = publisher1,
                Genres = new List<Genre> { genre1 },
                Platforms = new List<Platform> { platform1 },
                Prices = new List<GamePrice> { price1 },
                RegionPermissions = new List<Region> { region1 }
            };

            var updateDto = new UpdateGameDTO(
                Id: 1,
                Title: "Updated Game",
                PublisherId: 1,
                Description: "Updated Description",
                GenreIds: new List<int> { 2 },
                PlatformIds: new List<int> { 2 },
                PriceDto: new UpdateGamePriceDTO(Value: 59.99m, Stock: 5),
                RegionPermissionIds: new List<int> { 2 }
            );

            // Act
            _mapper.Map(updateDto, existingGame);

            // Assert
            Assert.Equal(originalDate, existingGame.CreatedAt);
            Assert.Equal(1, existingGame.PublisherId);
            Assert.Equal(2, existingGame.Publisher.Id);
            Assert.Equal("Original Publisher", existingGame.Publisher.Name);
            Assert.Single(existingGame.Genres);
            Assert.Equal(genre1, existingGame.Genres.First());
            Assert.Single(existingGame.Platforms);
            Assert.Equal(platform1, existingGame.Platforms.First());
            Assert.Single(existingGame.Prices);
            Assert.Equal(price1, existingGame.Prices.First());
            Assert.Single(existingGame.RegionPermissions);
            Assert.Equal(region1, existingGame.RegionPermissions.First());
        }

        [Fact]
        public void Maps_UpdateGameDTO_To_Game_Updates_UpdatedAt()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Test Publisher" };
            var genre = new Genre { Id = 1, Name = "RPG" };
            var platform = new Platform { Id = 1, Name = "PC" };
            var region = new Region { Id = 1, Name = "NA" };
            var price = new GamePrice
            {
                Id = 1,
                Value = 49.99m,
                Stock = 1,
                StartDate = DateTime.UtcNow.AddDays(-2),
                EndDate = null,
                Game = null!
            };
            var originalDate = DateTime.UtcNow.AddDays(-10);

            var existingGame = new Game
            {
                Id = 1,
                Title = "Test Game",
                Description = "Test Description",
                PublisherId = 1,
                CreatedAt = originalDate,
                UpdatedAt = DateTime.Now,
                Publisher = publisher,
                Genres = new List<Genre> { genre },
                Platforms = new List<Platform> { platform },
                Prices = new List<GamePrice> { price },
                RegionPermissions = new List<Region> { region }
            };

            var updateDto = new UpdateGameDTO(
                Id: 1,
                Title: "Updated Game",
                PublisherId: 1,
                Description: "Updated Description",
                GenreIds: new List<int> { 2 },
                PlatformIds: new List<int> { 2 },
                PriceDto: new UpdateGamePriceDTO(Value: 59.99m, Stock: 5),
                RegionPermissionIds: new List<int> { 2 }
            );

            // Act
            _mapper.Map(updateDto, existingGame);

            // Assert
            Assert.Equal("Updated Game", existingGame.Title);
            Assert.NotEqual(originalDate, existingGame.UpdatedAt);
            Assert.True((DateTime.UtcNow - existingGame.UpdatedAt).TotalSeconds < 1);
        }

        [Fact]
        public void Maps_UpdateGamePriceDTO_To_GamePrice()
        {
            // Arrange
            var updateDto = new UpdateGamePriceDTO(
                Value: 59.99m,
                Stock: 10
            );

            // Act
            var price = _mapper.Map<GamePrice>(updateDto);

            // Assert
            Assert.Equal(0, price.Id);
            Assert.Equal(updateDto.Value, price.Value);
            Assert.Equal(updateDto.Stock, price.Stock);
            Assert.True(price.StartDate > DateTime.MinValue);
            Assert.Null(price.EndDate);
        }
    }
}