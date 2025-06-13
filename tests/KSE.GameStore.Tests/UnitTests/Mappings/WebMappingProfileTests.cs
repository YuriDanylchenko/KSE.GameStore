using AutoMapper;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.Web.Mapping;
using KSE.GameStore.Web.Requests.Games;

namespace KSE.GameStore.Tests.UnitTests.Mappings;

public class WebMappingProfileTests
{
    private readonly IMapper _mapper;

    public WebMappingProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AddProfile<WebMappingProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Configuration_IsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    public class CreateGameRequestMapping : WebMappingProfileTests
    {
        [Fact]
        public void Maps_BasicProperties()
        {
            // Arrange
            var request = new CreateGameRequest(
                Title: "Test Game",
                Description: "Test Description",
                PublisherId: 1,
                GenreIds: new List<int> { 1, 2 },
                PlatformIds: new List<int> { 3 },
                new CreateGamePriceRequest(59.99m, 10),
                RegionPermissionIds: new List<int> { 4, 5 });

            // Act
            var dto = _mapper.Map<GameDTO>(request);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(0, dto.Id);
            Assert.Equal("Test Game", dto.Title);
            Assert.Equal("Test Description", dto.Description);
            Assert.Empty(dto.Publisher.Name);
            Assert.Equal(1, dto.Publisher.Id);
            Assert.Equal(2, dto.Genres.Count);
            Assert.Equal(1, dto.Genres[0].Id);
            Assert.Equal(2, dto.Genres[1].Id);
            Assert.All(dto.Genres, g => Assert.Empty(g.Name));
            Assert.Contains(dto.Platforms, p => p.Id == 3);
            Assert.All(dto.Platforms, p => Assert.Empty(p.Name));
            Assert.NotNull(dto.Price);
            Assert.Equal(59.99m, dto.Price.Value);
            Assert.Equal(10, dto.Price.Stock);
            Assert.Equal(2, dto.RegionPermissions!.Count);
            Assert.Equal(4, dto.RegionPermissions[0].Id);
            Assert.Equal(5, dto.RegionPermissions[1].Id);
            Assert.All(dto.RegionPermissions, r => Assert.True(string.IsNullOrEmpty(r.Name)));
        }


        [Fact]
        public void Maps_NullRegionPermissionsToNull()
        {
            // Arrange
            var request = new CreateGameRequest(
                Title: "Test Game",
                Description: "Test Description",
                PublisherId: 1,
                GenreIds: new List<int> { 1, 2 },
                PlatformIds: new List<int> { 3 },
                new CreateGamePriceRequest(59.99m, 10),
                RegionPermissionIds: null
            );

            // Act
            var dto = _mapper.Map<GameDTO>(request);

            // Assert
            Assert.Null(dto.RegionPermissions);
        }

        [Fact]
        public void Maps_EmptyRegionPermissionsToEmptyList()
        {
            // Arrange
            var request = new CreateGameRequest(
                Title: "Test Game",
                Description: "Test Description",
                PublisherId: 1,
                GenreIds: new List<int> { 1, 2 },
                PlatformIds: new List<int> { 3 },
                new CreateGamePriceRequest(59.99m, 10),
                RegionPermissionIds: new List<int>()
            );

            // Act
            var dto = _mapper.Map<GameDTO>(request);

            // Assert
            Assert.NotNull(dto.RegionPermissions);
            Assert.Empty(dto.RegionPermissions);
        }
    }

    public class CreateGamePriceRequestMapping : WebMappingProfileTests
    {
        [Fact]
        public void Maps_BasicProperties()
        {
            // Arrange
            var request = new CreateGamePriceRequest(59.99m, 10);

            // Act
            var dto = _mapper.Map<GamePriceDTO>(request);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(0, dto.Id);
            Assert.Equal(59.99m, dto.Value);
            Assert.Equal(10, dto.Stock);
        }
    }

    public class UpdateMappings : WebMappingProfileTests
    {
        [Fact]
        public void Maps_BasicProperties()
        {
            // Arrange
            var request = new UpdateGameRequest(
                Id: 1,
                Title: "Test Game",
                Description: "Test Description",
                PublisherId: 1,
                GenreIds: new List<int> { 1, 2 },
                PlatformIds: new List<int> { 3 },
                new UpdateGamePriceRequest(59.99m, 10),
                RegionPermissionIds: new List<int> { 4, 5 });

            // Act
            var dto = _mapper.Map<GameDTO>(request);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(1, dto.Id);
            Assert.Equal("Test Game", dto.Title);
            Assert.Equal("Test Description", dto.Description);
            Assert.Empty(dto.Publisher.Name);
            Assert.Equal(1, dto.Publisher.Id);
            Assert.Equal(2, dto.Genres.Count);
            Assert.Equal(1, dto.Genres[0].Id);
            Assert.Equal(2, dto.Genres[1].Id);
            Assert.All(dto.Genres, g => Assert.Empty(g.Name));
            Assert.Contains(dto.Platforms, p => p.Id == 3);
            Assert.All(dto.Platforms, p => Assert.Empty(p.Name));
            Assert.NotNull(dto.Price);
            Assert.Equal(59.99m, dto.Price.Value);
            Assert.Equal(10, dto.Price.Stock);
            Assert.Equal(2, dto.RegionPermissions!.Count);
            Assert.Equal(4, dto.RegionPermissions[0].Id);
            Assert.Equal(5, dto.RegionPermissions[1].Id);
            Assert.All(dto.RegionPermissions, r => Assert.True(string.IsNullOrEmpty(r.Name)));
        }

        [Fact]
        public void Maps_NullRegionPermissionsToNull()
        {
            // Arrange
            var request = new UpdateGameRequest(
                Id: 1,
                Title: "Test Game",
                Description: "Test Description",
                PublisherId: 1,
                GenreIds: new List<int> { 1, 2 },
                PlatformIds: new List<int> { 3 },
                new UpdateGamePriceRequest(59.99m, 10),
                RegionPermissionIds: null
            );

            // Act
            var dto = _mapper.Map<GameDTO>(request);

            // Assert
            Assert.Null(dto.RegionPermissions);
        }

        [Fact]
        public void Maps_EmptyRegionPermissionsToEmptyList()
        {
            // Arrange
            var request = new UpdateGameRequest(
                Id: 1,
                Title: "Test Game",
                Description: "Test Description",
                PublisherId: 1,
                GenreIds: new List<int> { 1, 2 },
                PlatformIds: new List<int> { 3 },
                new UpdateGamePriceRequest(59.99m, 10),
                RegionPermissionIds: new List<int>()
            );

            // Act
            var dto = _mapper.Map<GameDTO>(request);

            // Assert
            Assert.NotNull(dto.RegionPermissions);
            Assert.Empty(dto.RegionPermissions);
        }
    }

    public class UpdateGamePriceRequestMapping : WebMappingProfileTests
    {
        [Fact]
        public void Maps_BasicProperties()
        {
            // Arrange
            var request = new UpdateGamePriceRequest(59.99m, 10);

            // Act
            var dto = _mapper.Map<GamePriceDTO>(request);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(0, dto.Id);
            Assert.Equal(59.99m, dto.Value);
            Assert.Equal(10, dto.Stock);
        }
    }
}