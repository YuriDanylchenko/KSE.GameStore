using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.Tests.Helpers;
using KSE.GameStore.Web.Requests.Games;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace KSE.GameStore.Tests.IntegrationTests.Controllers;

public class GamesControllerTests : BaseIntegrationTest
{
    public GamesControllerTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    private async Task<(int PublisherId, int GenreId, int PlatformId)> CreateTestEntitiesAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreDbContext>();
        return await TestHelper.CreateTestEntitiesAsync(dbContext);
    }

    // ============ GET Tests ============
    [Fact]
    public async Task GetAll_ReturnsEmptyList_Initially()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var response = await Client.GetAsync("/games");

        response.EnsureSuccessStatusCode();
        var games = await response.Content.ReadFromJsonAsync<List<GameDTO>>();
        Assert.NotNull(games);
        Assert.Empty(games);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_ForInvalidId()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var response = await Client.GetAsync("/games/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ============ POST Tests ============
    [Fact]
    public async Task Create_And_GetById()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");

        // First create test entities
        var testIds = await CreateTestEntitiesAsync();

        // Create game
        var newGame = new CreateGameRequest(
            Title: "Test Game",
            Description: "Test Description",
            PublisherId: testIds.PublisherId,
            GenreIds: new List<int> { testIds.GenreId },
            PlatformIds: new List<int> { testIds.PlatformId },
            new CreateGamePriceRequest(59.99m, 10),
            RegionPermissionIds: null);

        var createResponse = await Client.PostAsJsonAsync("/games", newGame);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var createdGame = await createResponse.Content.ReadFromJsonAsync<GameDTO>();
        Assert.NotNull(createdGame);
        Assert.Equal("Test Game", createdGame.Title);

        // Verify we can fetch it
        var getResponse = await Client.GetAsync($"/games/{createdGame.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<GameDTO>();
        Assert.Equal(createdGame.Id, fetched!.Id);
        Assert.Equal("Test Game", fetched.Title);
        Assert.Equal("Test Description", fetched.Description);
        Assert.Equal("Test Publisher", fetched.Publisher.Name);
        Assert.Equal("Test Genre", fetched.Genres[0].Name);
        Assert.Equal("Test Platform", fetched.Platforms[0].Name);
        Assert.Equal(59.99m, fetched.Price.Value);
        Assert.Equal(10, fetched.Price.Stock);
        Assert.Empty(fetched.RegionPermissions!);
    }

    // ============ PUT Tests ============
    [Fact]
    public async Task Update_ModifiesExistingGame()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");

        // Setup - Create a game first
        var testIds = await CreateTestEntitiesAsync();

        var createRequest = new CreateGameRequest(
            Title: "Test Game",
            Description: "Test Description",
            PublisherId: testIds.PublisherId,
            GenreIds: new List<int> { testIds.GenreId },
            PlatformIds: new List<int> { testIds.PlatformId },
            new CreateGamePriceRequest(59.99m, 10),
            RegionPermissionIds: null);

        var createResponse = await Client.PostAsJsonAsync("/games", createRequest);
        var createdGame = (await createResponse.Content.ReadFromJsonAsync<GameDTO>())!;

        // Update the game
        var updateRequest = new UpdateGameRequest(
            createdGame.Id,
            "Updated Game Title",
            "Updated Description",
            testIds.PublisherId,
            new List<int> { testIds.GenreId },
            new List<int> { testIds.PlatformId },
            new UpdateGamePriceRequest(79.99m, 5),
            null);

        var updateResponse = await Client.PutAsJsonAsync("/games", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // Verify the update
        var getResponse = await Client.GetAsync($"/games/{createdGame.Id}");
        var updatedGame = await getResponse.Content.ReadFromJsonAsync<GameDTO>();
        Assert.Equal("Updated Game Title", updatedGame!.Title);
        Assert.Equal("Updated Description", updatedGame.Description);
        Assert.Equal(79.99m, updatedGame.Price.Value);
        Assert.Equal(5, updatedGame.Price.Stock);
    }

    // ============ DELETE Tests ============
    [Fact]
    public async Task Delete_RemovesGame()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");

        // Setup - Create a game
        var testIds = await CreateTestEntitiesAsync();

        var createRequest = new CreateGameRequest(
            Title: "Test Game",
            Description: "Test Description",
            PublisherId: testIds.PublisherId,
            GenreIds: new List<int> { testIds.GenreId },
            PlatformIds: new List<int> { testIds.PlatformId },
            new CreateGamePriceRequest(59.99m, 10),
            RegionPermissionIds: null);

        var createResponse = await Client.PostAsJsonAsync("/games", createRequest);
        var createdId = (await createResponse.Content.ReadFromJsonAsync<GameDTO>())!.Id;

        // Delete the game
        var deleteResponse = await Client.DeleteAsync($"/games/{createdId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify deletion
        var getResponse = await Client.GetAsync($"/games/{createdId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    // ============ Error Cases ============
    [Fact]
    public async Task Update_ReturnsNotFound_ForInvalidId()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");

        var testIds = await CreateTestEntitiesAsync();

        var updateRequest = new UpdateGameRequest(
            Id: 9999,
            Title: "Test Game",
            Description: "Test Description",
            PublisherId: testIds.PublisherId,
            GenreIds: new List<int> { testIds.GenreId },
            PlatformIds: new List<int> { testIds.PlatformId },
            new UpdateGamePriceRequest(59.99m, 10),
            RegionPermissionIds: null);

        var response = await Client.PutAsJsonAsync("/games", updateRequest);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_ForInvalidId()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var response = await Client.DeleteAsync("/games/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ============ GET By Genre Tests ============
    [Fact]
    public async Task GetGamesByGenre_ReturnsNotFound_ForInvalidGenre()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var response = await Client.GetAsync("/genre/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetGamesByGenre_ReturnsGames_ForValidGenre()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");

        // Create test entities and a game
        var testIds = await CreateTestEntitiesAsync();

        var newGame = new CreateGameRequest(
            Title: "Test Game",
            Description: "Test Description",
            PublisherId: testIds.PublisherId,
            GenreIds: new List<int> { testIds.GenreId },
            PlatformIds: new List<int> { testIds.PlatformId },
            new CreateGamePriceRequest(59.99m, 10),
            RegionPermissionIds: null);

        await Client.PostAsJsonAsync("/games", newGame);

        var response = await Client.GetAsync($"/genre/{testIds.GenreId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var games = await response.Content.ReadFromJsonAsync<List<GameDTO>>();
        Assert.NotNull(games);
        Assert.NotEmpty(games);
    }

    // ============ GET By Platform Tests ============
    [Fact]
    public async Task GetGamesByPlatform_ReturnsNotFound_ForInvalidPlatform()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var response = await Client.GetAsync("/platform/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetGamesByPlatform_ReturnsGames_ForValidPlatform()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");

        // Create test entities and a game
        var testIds = await CreateTestEntitiesAsync();

        var newGame = new CreateGameRequest(
            Title: "Test Game",
            Description: "Test Description",
            PublisherId: testIds.PublisherId,
            GenreIds: new List<int> { testIds.GenreId },
            PlatformIds: new List<int> { testIds.PlatformId },
            new CreateGamePriceRequest(59.99m, 10),
            RegionPermissionIds: null);

        await Client.PostAsJsonAsync("/games", newGame);

        var response = await Client.GetAsync($"/platform/{testIds.PlatformId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var games = await response.Content.ReadFromJsonAsync<List<GameDTO>>();
        Assert.NotNull(games);
        Assert.NotEmpty(games);
    }
}
