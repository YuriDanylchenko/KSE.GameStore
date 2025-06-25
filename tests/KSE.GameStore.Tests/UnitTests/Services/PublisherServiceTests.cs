using System.Linq.Expressions;
using AutoMapper;
using KSE.GameStore.ApplicationCore.Mapping;
using KSE.GameStore.ApplicationCore.Models.Input;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using KSE.GameStore.Web.Mapping;
using Microsoft.Extensions.Logging;
using Moq;

namespace KSE.GameStore.Tests.UnitTests.Services;

public class PublisherServiceTests
{
    private readonly Mock<IRepository<Publisher, int>> _mockRepo;
    private readonly Mock<ILogger<PublisherService>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly PublisherService _service;

    #region Constructor

    public PublisherServiceTests()
    {
        _mockRepo = new Mock<IRepository<Publisher, int>>();
        _mockLogger = new Mock<ILogger<PublisherService>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AddProfile<WebMappingProfile>();
            cfg.AddProfile<ApplicationCoreMappingProfile>();
        });
        _mapper = config.CreateMapper();

        _service = new PublisherService(
            _mockRepo.Object,
            _mockLogger.Object,
            _mapper);
    }

    #endregion

    #region GetAll Tests

    public class GetAllPublishers : PublisherServiceTests
    {
        [Fact]
        public async Task GetAllPublishersAsync_ReturnsMappedList()
        {
            // Arrange
            var publishers = new List<Publisher>
            {
                new() { Id = 1, Name = "P1", WebsiteUrl = "url1", Description = "desc1" },
                new() { Id = 2, Name = "P2", WebsiteUrl = "url2", Description = "desc2" },
            };

            _mockRepo.Setup(r => r.ListAsync(1, 10)).ReturnsAsync(publishers);

            // Act
            var result = await _service.GetAllPublishersAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("P1", result[0].Name);
            Assert.Equal("P2", result[1].Name);
        }

        [Fact]
        public async Task GetAllPublishersAsync_ThrowsBadRequest_WhenPageNumberIsZero()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(() => _service.GetAllPublishersAsync(0, 10));
            Assert.Contains("Page number", ex.Message);
        }

        [Fact]
        public async Task GetAllPublishersAsync_ThrowsBadRequest_WhenPageSizeIsZero()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(() => _service.GetAllPublishersAsync(1, 0));
            Assert.Contains("Page size", ex.Message);
        }

        [Fact]
        public async Task GetAllPublishersAsync_UsesDefaultPaging_WhenNullsProvided()
        {
            // Arrange
            var publishers = new List<Publisher> { new() { Id = 1, Name = "Default" } };

            _mockRepo.Setup(r => r.ListAsync(1, 10)).ReturnsAsync(publishers);

            // Act
            var result = await _service.GetAllPublishersAsync(null, null);

            // Assert
            Assert.Single(result);
            Assert.Equal("Default", result.First().Name);
        }
    }

    #endregion

    #region GetById Tests

    public class GetPublisherById : PublisherServiceTests
    {
        [Fact]
        public async Task GetPublisherByIdAsync_ThrowsNotFound_WhenPublisherMissing()
        {
            // Act
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Publisher)null!);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetPublisherByIdAsync(1));
        }

        [Fact]
        public async Task GetPublisherByIdAsync_ReturnsPublisher_WhenExists()
        {
            // Arrange
            var publisher = new Publisher
            {
                Id = 1,
                Name = "Test Publisher",
                WebsiteUrl = "http://example.com",
                Description = "Sample description"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(publisher);

            // Act
            var result = await _service.GetPublisherByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(publisher.Id, result.Id);
            Assert.Equal(publisher.Name, result.Name);
            Assert.Equal(publisher.WebsiteUrl, result.WebsiteUrl);
            Assert.Equal(publisher.Description, result.Description);
        }
    }

    #endregion

    #region Create Tests

    public class CreatePublisher : PublisherServiceTests
    {
        [Fact]
        public async Task CreatePublisherAsync_ThrowsBadRequest_WhenDuplicateName()
        {
            // Arrange
            var dto = new CreatePublisherDTO
            (
                "Test Game",
                "https://new.com",
                "desc"
            );

            var existingPublishers = new List<Publisher> { new() { Name = "Test Game" } };

            _mockRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Publisher, bool>>>()))
                .ReturnsAsync(existingPublishers);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreatePublisherAsync(dto));
        }

        [Fact]
        public async Task CreatePublisherAsync_CreatesPublisherSuccessfully()
        {
            // Arrange
            var dto = new CreatePublisherDTO
            (
                "New Publisher",
                "https://new.com",
                "new desc"
            );

            _mockRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Publisher, bool>>>()))
                .ReturnsAsync(new List<Publisher>());

            Publisher? addedPublisher = null;
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Publisher>()))
                .Callback<Publisher>(pub => addedPublisher = pub)
                .Returns(Task.CompletedTask);

            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreatePublisherAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Name, addedPublisher?.Name);
            Assert.Equal(dto.Description, addedPublisher?.Description);
            Assert.Equal(dto.WebsiteUrl, addedPublisher?.WebsiteUrl);
        }
    }

    #endregion

    #region Update Tests

    public class UpdatePublisher : PublisherServiceTests
    {
        [Fact]
        public async Task UpdatePublisherAsync_ThrowsNotFound_WhenPublisherMissing()
        {
            // Arrange
            var dto = new UpdatePublisherDTO(
                1,
                "Updated",
                "url",
                "desc"
            );

            _mockRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Publisher)null!);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdatePublisherAsync(dto));
        }

        [Fact]
        public async Task UpdatePublisherAsync_ThrowsBadRequest_WhenNameAlreadyExists()
        {
            // Arrange
            var dto = new UpdatePublisherDTO(
                1,
                "Updated Name",
                "url",
                "desc"
            );
            var existingPublisher = new Publisher { Id = 1, Name = "Old Name", Description = "desc", WebsiteUrl = "url" };

            _mockRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(existingPublisher);
            _mockRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Publisher, bool>>>()))
                .ReturnsAsync(new List<Publisher> { new() { Id = 2, Name = "Updated Name" } });

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdatePublisherAsync(dto));
        }

        [Fact]
        public async Task UpdatePublisherAsync_UpdatesSuccessfully()
        {
            // Arrange
            var dto = new UpdatePublisherDTO(
                1,
                "Updated Name",
                "new url",
                "new desc"
            );

            var existingPublisher = new Publisher { Id = 1, Name = "Old Name", Description = "desc", WebsiteUrl = "url" };

            _mockRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(existingPublisher);
            _mockRepo.Setup(r => r.ListAllAsync(It.IsAny<Expression<Func<Publisher, bool>>>()))
                .ReturnsAsync(new List<Publisher>());

            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.Update(It.IsAny<Publisher>()));
            _mockRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(existingPublisher);

            // Act
            var result = await _service.UpdatePublisherAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(dto.WebsiteUrl, result.WebsiteUrl);
        }
    }

    #endregion

    #region Delete Tests

    public class DeletePublisher : PublisherServiceTests
    {
        [Fact]
        public async Task DeletePublisherAsync_ThrowsNotFound_WhenPublisherMissing()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Publisher)null!);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeletePublisherAsync(1));
        }

        [Fact]
        public async Task DeletePublisherAsync_DeletesSuccessfully()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "ToDelete" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(publisher);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.DeletePublisherAsync(1);

            // Assert
            _mockRepo.Verify(r => r.Delete(publisher), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }

    #endregion
}