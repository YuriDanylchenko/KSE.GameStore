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
        _service = new GenreService(_mockRepo.Object, new Random(0));
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
    public async Task GetGenreByIdAsync_ReturnsNull_WhenIdInvalid()
    {
        var result = await _service.GetGenreByIdAsync(0);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateGenreAsync_ReturnsNull_WhenNameIsEmpty()
    {
        var result = await _service.CreateGenreAsync("");
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateGenreAsync_ReturnsNull_WhenGenreExists()
    {
        _mockRepo.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<DataAccess.Entities.Genre, bool>>>(), 1, 10))
            .ReturnsAsync(new List<DataAccess.Entities.Genre> { new() { Name = "Adventure" } });

        var result = await _service.CreateGenreAsync("Adventure");
        Assert.Null(result);
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
    public async Task UpdateGenreAsync_ReturnsNull_WhenGenreNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((DataAccess.Entities.Genre)null!);

        var result = await _service.UpdateGenreAsync(42, "RPG");

        Assert.Null(result);
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
    public async Task DeleteGenreAsync_ReturnsFalse_WhenGenreNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(77)).ReturnsAsync((DataAccess.Entities.Genre)null!);

        var result = await _service.DeleteGenreAsync(77);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteGenreAsync_ReturnsTrue_WhenGenreExists()
    {
        var genre = new DataAccess.Entities.Genre { Id = 99, Name = "Test" };
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync(genre);

        var result = await _service.DeleteGenreAsync(99);

        Assert.True(result);
    }
}