using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace KSE.GameStore.Tests.UnitTests.Services;

public class PlatformsServiceTests
{
    private static GameStoreDbContext CreateDbContext(string dbName) => new(
        new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options);

    private static PlatformsService CreatePlatformsService(GameStoreDbContext context) => new(
        new Repository<Platform, int>(context),
        new Mock<ILogger<PlatformsService>>().Object);

    [Fact]
    public async Task GetAll_ReturnsAllPlatforms()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        context.Platforms.Add(new Platform { Name = "PC" });
        context.Platforms.Add(new Platform { Name = "Xbox" });
        await context.SaveChangesAsync();

        var service = CreatePlatformsService(context);
        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "PC");
        Assert.Contains(result, p => p.Name == "Xbox");
    }

    [Fact]
    public async Task GetById_ReturnsPlatform_WhenExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var platform = new Platform { Name = "PC" };
        context.Platforms.Add(platform);
        await context.SaveChangesAsync();

        var service = CreatePlatformsService(context);

        var result = await service.GetByIdAsync(platform.Id);

        Assert.NotNull(result);
        Assert.Equal("PC", result!.Name);
    }

    [Fact]
    public async Task GetById_ThrowsNotFoundException_WhenNotExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var service = CreatePlatformsService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(99));
        Assert.Equal("Platform with id 99 not found.", ex.Message);
    }

    [Fact]
    public async Task Create_AddsPlatform()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = CreatePlatformsService(context);

        var result = await service.CreateAsync("Switch");

        Assert.Single(context.Platforms);
        Assert.Equal("Switch", context.Platforms.First().Name);
    }

    [Fact]
    public async Task Update_UpdatesPlatform_WhenExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var platform = new Platform { Name = "Old" };
        context.Platforms.Add(platform);
        await context.SaveChangesAsync();

        var service = CreatePlatformsService(context);

        var updated = await service.UpdateAsync(platform.Id, "New");

        Assert.True(updated);
        Assert.Equal("New", context.Platforms.Find(platform.Id)!.Name);
    }

    [Fact]
    public async Task Update_ThrowsNotFoundException_WhenNotExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = CreatePlatformsService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(1, "New"));
        Assert.Equal("Platform with id 1 not found.", ex.Message);
    }

    [Fact]
    public async Task Delete_RemovesPlatform_WhenExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var platform = new Platform { Name = "PC" };
        context.Platforms.Add(platform);
        await context.SaveChangesAsync();

        var service = CreatePlatformsService(context);

        var deleted = await service.DeleteAsync(platform.Id);

        Assert.True(deleted);
        Assert.Empty(context.Platforms);
    }

    [Fact]
    public async Task Delete_ThrowsNotFoundException_WhenNotExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = CreatePlatformsService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(1));
        Assert.Equal("Platform with id 1 not found.", ex.Message);
    }
}