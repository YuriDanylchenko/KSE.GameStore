using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Mapping.Games;
using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace KSE.GameStore.Tests.IntegrationTests;

public class GamesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GamesControllerTests(WebApplicationFactory<Program> factory)
    {
        var dbName = $"GamesTestDb-{Guid.NewGuid()}";
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("IntegrationTest");
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext configuration
                var dbContext = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<GameStoreDbContext>));
                if (dbContext != null)
                    services.Remove(dbContext);

                // Add in-memory database
                services.AddDbContext<GameStoreDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
            });
        });
    }
    
    private async Task<int> CreateTestPublisher(HttpClient client)
    {
        // Get the DbContext from the factory
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreDbContext>();
    
        // Create and save a publisher
        var publisher = new Publisher { Name = "Test Publisher" };
        dbContext.Publishers.Add(publisher);
        await dbContext.SaveChangesAsync();
    
        return publisher.Id;
    }

    // ============ GET Tests ============
    [Fact]
    public async Task GetAll_ReturnsEmptyList_Initially()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/games");
        
        response.EnsureSuccessStatusCode();
        var games = await response.Content.ReadFromJsonAsync<List<GameDTO>>();
        Assert.NotNull(games);
        Assert.Empty(games);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_ForInvalidId()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/games/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ============ POST Tests ============
    [Fact]
    public async Task Create_And_GetById()
    {
        var client = _factory.CreateClient();
        
        // First create a publisher (required relationship)
        var publisherId = await CreateTestPublisher(client);

        // Create game
        var newGame = new CreateGameRequest(
            Title: "Test Game",
            Description: "Test Description",
            PublisherId: publisherId,
            GenreIds: new List<int>(),
            PlatformIds: new List<int>(),
            new CreateGamePriceRequest(59.99m, 10),
            RegionPermissionIds: null);

        var createResponse = await client.PostAsJsonAsync("/games", newGame);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var createdGame = await createResponse.Content.ReadFromJsonAsync<GameDTO>();
        Assert.NotNull(createdGame);
        Assert.Equal("Test Game", createdGame.Title);

        // Verify we can fetch it
        var getResponse = await client.GetAsync($"/games/{createdGame.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<GameDTO>();
        Assert.Equal(createdGame.Id, fetched!.Id);
        Assert.Equal("Test Game", fetched.Title);
        Assert.Equal("Test Description", fetched.Description);
        Assert.Equal("Test Publisher", fetched.Publisher);
        Assert.Equal(59.99m, fetched.Price.Value);
        Assert.Equal(10, fetched.Price.Stock);
    }

    // ============ PUT Tests ============
    [Fact]
    public async Task Update_ModifiesExistingGame()
    {
        var client = _factory.CreateClient();
        
        // Setup - Create a game first
        var publisherId = await CreateTestPublisher(client);

        var createRequest = new CreateGameRequest(
            Title: "Test Game",
            Description: "Test Description",
            PublisherId: publisherId,
            GenreIds: new List<int>(),
            PlatformIds: new List<int>(),
            new CreateGamePriceRequest(59.99m, 10),
            RegionPermissionIds: null);
        
        var createResponse = await client.PostAsJsonAsync("/games", createRequest);
        var createdId = (await createResponse.Content.ReadFromJsonAsync<GameDTO>())!.Id;

        // Update the game
        var updateRequest = new UpdateGameRequest(
            Id: createdId,
            Title: "Updated Test Game",
            Description: "Updated Test Description",
            PublisherId: publisherId,
            GenreIds: new List<int>(),
            PlatformIds: new List<int>(),
            new UpdateGamePriceRequest(69.99m, 11),
            RegionPermissionIds: null);

        var updateResponse = await client.PutAsJsonAsync("/games", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // Verify update
        var getResponse = await client.GetAsync($"/games/{createdId}");
        var updated = await getResponse.Content.ReadFromJsonAsync<GameDTO>();
        Assert.Equal("Updated Test Game", updated!.Title);
        Assert.Equal("Updated Test Description", updated.Description);
        Assert.Equal(69.99m, updated.Price.Value);
        Assert.Equal(11, updated.Price.Stock);
    }

    // ============ DELETE Tests ============
    [Fact]
    public async Task Delete_RemovesGame()
    {
        var client = _factory.CreateClient();
        
        // Setup - Create a game
        var publisherId = await CreateTestPublisher(client);

        var createRequest = new CreateGameRequest(
            Title: "Test Game",
            Description: "Test Description",
            PublisherId: publisherId,
            GenreIds: new List<int>(),
            PlatformIds: new List<int>(),
            new CreateGamePriceRequest(59.99m, 10),
            RegionPermissionIds: null);

        
        var createResponse = await client.PostAsJsonAsync("/games", createRequest);
        var createdId = (await createResponse.Content.ReadFromJsonAsync<GameDTO>())!.Id;

        // Delete the game
        var deleteResponse = await client.DeleteAsync($"/games/{createdId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify deletion
        var getResponse = await client.GetAsync($"/games/{createdId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    // ============ Error Cases ============
    [Fact]
    public async Task Update_ReturnsNotFound_ForInvalidId()
    {
        var client = _factory.CreateClient();
        
        var publisherId = await CreateTestPublisher(client);
        
        var updateRequest = new UpdateGameRequest(
            Id: 9999,
            Title: "Test Game",
            Description: "Test Description",
            PublisherId: publisherId,
            GenreIds: new List<int>(),
            PlatformIds: new List<int>(),
            new UpdateGamePriceRequest(59.99m, 10),
            RegionPermissionIds: null);
        
        var response = await client.PutAsJsonAsync("/games", updateRequest);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_ForInvalidId()
    {
        var client = _factory.CreateClient();
        var response = await client.DeleteAsync("/games/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ============ Pagination Tests ============
    [Fact]
    public async Task GetAll_SupportsPagination()
    {
        var client = _factory.CreateClient();
        
        // Create test data
        for (int i = 1; i <= 5; i++)
        {
            var publisherId = await CreateTestPublisher(client);

            var game = new CreateGameRequest(
                Title: $"Game {i}",
                Description: $"Description for Game {i}",
                PublisherId: publisherId,
                GenreIds: new List<int>(),
                PlatformIds: new List<int>(),
                new CreateGamePriceRequest(59.99m, 10),
                RegionPermissionIds: null);
            
            await client.PostAsJsonAsync("/games", game);
        }

        // Test pagination
        var page1 = await client.GetFromJsonAsync<List<GameDTO>>("/games?pageNumber=1&pageSize=2");
        Assert.Equal(2, page1!.Count);
        Assert.Equal("Game 1", page1[0].Title);

        var page2 = await client.GetFromJsonAsync<List<GameDTO>>("/games?pageNumber=2&pageSize=2");
        Assert.Equal(2, page2!.Count);
        Assert.Equal("Game 3", page2[0].Title);
    }
}
