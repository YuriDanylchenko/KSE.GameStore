using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using KSE.GameStore.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace KSE.GameStore.Tests.Services;

public class PlatformsServiceTests
{
    private static GameStoreDbContext CreateDbContext(string dbName) => new(
        new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options);

    [Fact]
    public async Task GetAll_ReturnsAllPlatforms()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        context.Platforms.Add(new Platform { Name = "PC" });
        context.Platforms.Add(new Platform { Name = "Xbox" });
        await context.SaveChangesAsync();

        var service = new PlatformsService(new Repository<Platform, int>(context));

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

        var service = new PlatformsService(new Repository<Platform, int>(context));

        var result = await service.GetByIdAsync(platform.Id);

        Assert.NotNull(result);
        Assert.Equal("PC", result!.Name);
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenNotExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var service = new PlatformsService(new Repository<Platform, int>(context));

        var result = await service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task Create_AddsPlatform()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new PlatformsService(new Repository<Platform, int>(context));

        var result = await service.CreateAsync("Switch");

        Assert.Single(context.Platforms);
        Assert.Equal("Switch", result.Name);
    }

    [Fact]
    public async Task Update_UpdatesPlatform_WhenExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var platform = new Platform { Name = "Old" };
        context.Platforms.Add(platform);
        await context.SaveChangesAsync();

        var service = new PlatformsService(new Repository<Platform, int>(context));

        var updated = await service.UpdateAsync(platform.Id, "New");

        Assert.True(updated);
        Assert.Equal("New", context.Platforms.Find(platform.Id)!.Name);
    }

    [Fact]
    public async Task Update_ReturnsFalse_WhenNotExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new PlatformsService(new Repository<Platform, int>(context));

        var updated = await service.UpdateAsync(1, "New" );

        Assert.False(updated);
    }

    [Fact]
    public async Task Delete_RemovesPlatform_WhenExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var platform = new Platform { Name = "PC" };
        context.Platforms.Add(platform);
        await context.SaveChangesAsync();

        var service = new PlatformsService(new Repository<Platform, int>(context));

        var deleted = await service.DeleteAsync(platform.Id);

        Assert.True(deleted);
        Assert.Empty(context.Platforms);
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenNotExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new PlatformsService(new Repository<Platform, int>(context));

        var deleted = await service.DeleteAsync(1);

        Assert.False(deleted);
    }
}