﻿using AutoMapper;
using KSE.GameStore.ApplicationCore.Mapping;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using KSE.GameStore.Web.Mapping;
using Moq;

namespace KSE.GameStore.Tests.UnitTests.Services;

public class GenreServiceTests
{
    private readonly Mock<IRepository<Genre, int>> _mockRepo;
    private readonly IMapper _mapper;
    private readonly GenreService _service;

    public GenreServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AddProfile<WebMappingProfile>();
            cfg.AddProfile<ApplicationCoreMappingProfile>();
        });
        _mapper = config.CreateMapper();
        _mockRepo = new Mock<IRepository<Genre, int>>();
        _service = new GenreService(_mapper, _mockRepo.Object);
    }

    [Fact]
    public async Task GetGenreByIdAsync_ReturnsGenre_WhenGenreExists()
    {
        // Arrange
        var genre = new Genre { Id = 1, Name = "Action", Games = new List<Game>() };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(genre);

        // Act
        var result = await _service.GetGenreByIdAsync(1);

        // Assert
        Assert.Equal(genre, result);
    }

    [Fact]
    public async Task GetGenreByIdAsync_ThrowsBadRequest_WhenIdInvalid()
    {
        // Act
        _mockRepo.Setup(r => r.GetByIdAsync(77)).ReturnsAsync((Genre)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetGenreByIdAsync(0));
    }

    [Fact]
    public async Task CreateGenreAsync_ThrowsBadRequest_WhenGenreExists()
    {
        // Arrange
        _mockRepo.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Genre, bool>>>(), 1, 10))
            .ReturnsAsync(new List<Genre> { new() { Name = "Adventure" } });

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateGenreAsync("Adventure"));
    }

    [Fact]
    public async Task CreateGenreAsync_ReturnsGenre_WhenGenreDoesNotExist()
    {
        // Arrange
        _mockRepo.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Genre, bool>>>(), 1, 10))
            .ReturnsAsync(new List<Genre>());
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Genre>())).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateGenreAsync("NewGenre");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NewGenre", result.Name);
    }

    [Fact]
    public async Task UpdateGenreAsync_ThrowsBadRequest_WhenIdInvalid()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateGenreAsync(0, "RPG"));
    }

    [Fact]
    public async Task UpdateGenreAsync_ThrowsNotFound_WhenGenreNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Genre)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateGenreAsync(42, "RPG"));
    }

    [Fact]
    public async Task UpdateGenreAsync_UpdatesName_WhenGenreExists()
    {
        // Arrange
        var genre = new Genre { Id = 7, Name = "Old" };
        _mockRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(genre);

        // Act
        var result = await _service.UpdateGenreAsync(7, "New");

        // Assert
        Assert.Equal("New", genre.Name);
    }

    [Fact]
    public async Task DeleteGenreAsync_ThrowsBadRequest_WhenIdInvalid()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteGenreAsync(0));
    }

    [Fact]
    public async Task DeleteGenreAsync_ThrowsNotFound_WhenGenreNotFound()
    {
        // Act
        _mockRepo.Setup(r => r.GetByIdAsync(77)).ReturnsAsync((Genre)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteGenreAsync(77));
    }

    [Fact]
    public async Task DeleteGenreAsync_ReturnsTrue_WhenGenreExists()
    {
        // Arrange
        var genre = new Genre { Id = 99, Name = "Test" };
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync(genre);
        _mockRepo.Setup(r => r.Delete(genre));
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteGenreAsync(99);

        // Assert
        Assert.True(result);
    }
}