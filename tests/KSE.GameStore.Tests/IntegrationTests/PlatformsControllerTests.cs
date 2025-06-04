using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace KSE.GameStore.Tests.IntegrationTests;

public class PlatformsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PlatformsControllerTests(WebApplicationFactory<Program> factory)
    {
        var dbName = $"IntegrationTestDb-{Guid.NewGuid()}";
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("IntegrationTest");
            builder.ConfigureServices(services =>
            {
                var dbContext = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<GameStoreDbContext>));
                if (dbContext != null)
                    services.Remove(dbContext);

                services.AddDbContext<GameStoreDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
            });
        });
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_Initially()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/platforms");
        response.EnsureSuccessStatusCode();

        var platforms = await response.Content.ReadFromJsonAsync<List<Platform>>();
        Assert.NotNull(platforms);
        Assert.Empty(platforms!);
    }

    [Fact]
    public async Task Create_And_GetById()
    {
        var client = _factory.CreateClient();
        var newPlatform = new Platform { Name = "PlayStation" };

        var createResponse = await client.PostAsJsonAsync("/platforms", newPlatform);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var createdId = await createResponse.Content.ReadFromJsonAsync<int>();
        Assert.True(createdId > 0);

        var getResponse = await client.GetAsync($"/platforms/{createdId}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<Platform>();
        Assert.NotNull(fetched);
        Assert.Equal("PlayStation", fetched.Name);
    }

    [Fact]
    public async Task Update()
    {
        var client = _factory.CreateClient();
        var platform = new Platform { Name = "Wii" };
        var createResponse = await client.PostAsJsonAsync("/platforms", platform);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var createdId = await createResponse.Content.ReadFromJsonAsync<int>();

        var updated = new Platform { Name = "Wii U" };
        var updateResponse = await client.PutAsJsonAsync($"/platforms/{createdId}", updated);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var getResponse = await client.GetAsync($"/platforms/{createdId}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<Platform>();
        Assert.Equal("Wii U", fetched!.Name);
    }

    [Fact]
    public async Task Delete()
    {
        var client = _factory.CreateClient();
        var platform = new Platform { Name = "Stadia" };
        var createResponse = await client.PostAsJsonAsync("/platforms", platform);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var createdId = await createResponse.Content.ReadFromJsonAsync<int>();

        var deleteResponse = await client.DeleteAsync($"/platforms/{createdId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/platforms/{createdId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Update_NotFound_Returns404()
    {
        var client = _factory.CreateClient();
        var updated = new Platform { Name = "NonExistent" };
        var response = await client.PutAsJsonAsync("/platforms/9999", updated);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NotFound_Returns404()
    {
        var client = _factory.CreateClient();
        var response = await client.DeleteAsync("/platforms/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}