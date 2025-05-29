using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using Moq;

namespace KSE.GameStore.Tests.Genre;

public class GenreServiceTests
{
    private readonly Mock<IRepository<DataAccess.Entities.Genre, int>> _mockRepo;
    private readonly GenreService _service;
    
    public GenreServiceTests()
    {
        _mockRepo = new Mock<IRepository<DataAccess.Entities.Genre, int>>();
        _service = new GenreService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetGenreByIdAsync_ReturnsGenre_WhenGenreExists()
    {
        // Arrange
        var genre = new DataAccess.Entities.Genre { Id = 1, Name = "Action", Games = new List<Game>() };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(genre);

        // Act
        var result = await _service.GetGenreByIdAsync(1);

        // Assert
        Assert.Equal(genre, result);
    }

    [Fact]
    public async Task GetGenreByIdAsync_ThrowsBadRequest_WhenIdInvalid()
    {
        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.GetGenreByIdAsync(0));
    }

    [Fact]
    public async Task CreateGenreAsync_ThrowsBadRequest_WhenNameIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateGenreAsync(""));
    }

    [Fact]
    public async Task CreateGenreAsync_ThrowsBadRequest_WhenGenreExists()
    {
        _mockRepo.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<DataAccess.Entities.Genre, bool>>>(), 1, 10))
            .ReturnsAsync(new List<DataAccess.Entities.Genre> { new() { Name = "Adventure" } });

        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateGenreAsync("Adventure"));
    }

    [Fact]
    public async Task CreateGenreAsync_ReturnsGenre_WhenGenreDoesNotExist()
    {
        _mockRepo.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<DataAccess.Entities.Genre, bool>>>(), 1, 10))
            .ReturnsAsync(new List<DataAccess.Entities.Genre>());
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<DataAccess.Entities.Genre>())).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateGenreAsync("NewGenre");

        Assert.NotNull(result);
        Assert.Equal("NewGenre", result.Name);
    }

    [Fact]
    public async Task UpdateGenreAsync_ThrowsBadRequest_WhenIdInvalid()
    {
        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateGenreAsync(0, "RPG"));
    }
    
    [Fact]
    public async Task UpdateGenreAsync_ThrowsBadRequest_WhenNameInvalid()
    {
        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateGenreAsync(10, ""));
    }

    [Fact]
    public async Task UpdateGenreAsync_ThrowsNotFound_WhenGenreNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((DataAccess.Entities.Genre)null!);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateGenreAsync(42, "RPG"));
    }
    
    [Fact]
    public async Task UpdateGenreAsync_UpdatesName_WhenGenreExists()
    {
        var genre = new DataAccess.Entities.Genre { Id = 7, Name = "Old" };
        _mockRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(genre);

        var result = await _service.UpdateGenreAsync(7, "New");

        Assert.Equal("New", genre.Name);
    }

    [Fact]
    public async Task DeleteGenreAsync_ThrowsBadRequest_WhenIdInvalid()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => _service.DeleteGenreAsync(0));
    }

    [Fact]
    public async Task DeleteGenreAsync_ThrowsNotFound_WhenGenreNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(77)).ReturnsAsync((DataAccess.Entities.Genre)null!);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteGenreAsync(77));
    }

    [Fact]
    public async Task DeleteGenreAsync_ReturnsTrue_WhenGenreExists()
    {
        var genre = new DataAccess.Entities.Genre { Id = 99, Name = "Test" };
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync(genre);
        _mockRepo.Setup(r => r.Delete(genre));
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.DeleteGenreAsync(99);

        Assert.True(result);
    }
}