using AutoMapper;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Models.Input;
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
            var dto = _mapper.Map<CreateGameDTO>(request);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal("Test Game", dto.Title);
            Assert.Equal("Test Description", dto.Description);
            Assert.Equal(1, dto.PublisherId);
            Assert.Equal(2, dto.GenreIds.Count);
            Assert.Equal(1, dto.GenreIds[0]);
            Assert.Equal(2, dto.GenreIds[1]);
            Assert.Contains(dto.PlatformIds, p => p == 3);
            Assert.NotNull(dto.PriceDto);
            Assert.Equal(59.99m, dto.PriceDto.Value);
            Assert.Equal(10, dto.PriceDto.Stock);
            Assert.Equal(2, dto.RegionPermissionIds!.Count);
            Assert.Equal(4, dto.RegionPermissionIds[0]);
            Assert.Equal(5, dto.RegionPermissionIds[1]);
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
            var dto = _mapper.Map<CreateGameDTO>(request);

            // Assert
            Assert.Null(dto.RegionPermissionIds);
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
            var dto = _mapper.Map<CreateGameDTO>(request);

            // Assert
            Assert.NotNull(dto.RegionPermissionIds);
            Assert.Empty(dto.RegionPermissionIds);
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
            var dto = _mapper.Map<CreateGamePriceDTO>(request);

            // Assert
            Assert.NotNull(dto);
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
            var dto = _mapper.Map<UpdateGameDTO>(request);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(1, dto.Id);
            Assert.Equal("Test Game", dto.Title);
            Assert.Equal("Test Description", dto.Description);
            Assert.Equal(1, dto.PublisherId);
            Assert.Equal(2, dto.GenreIds.Count);
            Assert.Equal(1, dto.GenreIds[0]);
            Assert.Equal(2, dto.GenreIds[1]);
            Assert.Contains(dto.PlatformIds, p => p == 3);
            Assert.NotNull(dto.PriceDto);
            Assert.Equal(59.99m, dto.PriceDto.Value);
            Assert.Equal(10, dto.PriceDto.Stock);
            Assert.Equal(2, dto.RegionPermissionIds!.Count);
            Assert.Equal(4, dto.RegionPermissionIds[0]);
            Assert.Equal(5, dto.RegionPermissionIds[1]);
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
            var dto = _mapper.Map<UpdateGameDTO>(request);

            // Assert
            Assert.Null(dto.RegionPermissionIds);
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
            var dto = _mapper.Map<UpdateGameDTO>(request);

            // Assert
            Assert.NotNull(dto.RegionPermissionIds);
            Assert.Empty(dto.RegionPermissionIds);
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
            var dto = _mapper.Map<UpdateGamePriceDTO>(request);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(59.99m, dto.Value);
            Assert.Equal(10, dto.Stock);
        }
    }
}