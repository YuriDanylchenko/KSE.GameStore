using AutoMapper;
using KSE.GameStore.ApplicationCore.Mapping;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using KSE.GameStore.Web.Mapping;
using Moq;

namespace KSE.GameStore.Tests.UnitTests.Services;

public class PlatformsServiceTests
{
    private readonly Mock<IRepository<Platform, int>> _mockRepo;
    private readonly IMapper _mapper;
    private readonly PlatformsService _service;

    public PlatformsServiceTests()
    {
        _mockRepo = new Mock<IRepository<Platform, int>>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AddProfile<WebMappingProfile>();
            cfg.AddProfile<ApplicationCoreMappingProfile>();
        });
        _mapper = config.CreateMapper();
        _service = new PlatformsService(_mockRepo.Object, _mapper);
    }

    [Fact]
    public async Task GetById_ReturnsPlatform_WhenExists()
    {
        var platform = new Platform { Id = 1, Name = "PC" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(platform);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("PC", result!.Name);
    }

    [Fact]
    public async Task GetById_ThrowsNotFoundException_WhenNotExists()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Platform?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
        Assert.Equal("Platform with id 99 not found.", ex.Message);
    }

    [Fact]
    public async Task Create_AddsPlatform()
    {
        Platform? added = null;
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Platform>()))
            .Callback<Platform>(p => { p.Id = 1; added = p; })
            .Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync("Switch");

        Assert.NotNull(added);
        Assert.Equal("Switch", added.Name);
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task Update_UpdatesPlatform_WhenExists()
    {
        var platform = new Platform { Id = 1, Name = "Old" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(platform);
        _mockRepo.Setup(r => r.Update(platform));
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var updated = await _service.UpdateAsync(1, "New");

        Assert.True(updated);
        Assert.Equal("New", platform.Name);
    }

    [Fact]
    public async Task Update_ThrowsNotFoundException_WhenNotExists()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Platform?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(1, "New"));
        Assert.Equal("Platform with id 1 not found.", ex.Message);
    }

    [Fact]
    public async Task Delete_RemovesPlatform_WhenExists()
    {
        var platform = new Platform { Id = 1, Name = "PC" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(platform);
        _mockRepo.Setup(r => r.Delete(platform));
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var deleted = await _service.DeleteAsync(1);

        Assert.True(deleted);
        _mockRepo.Verify(r => r.Delete(platform), Times.Once);
    }

    [Fact]
    public async Task Delete_ThrowsNotFoundException_WhenNotExists()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Platform?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(1));
        Assert.Equal("Platform with id 1 not found.", ex.Message);
    }
}