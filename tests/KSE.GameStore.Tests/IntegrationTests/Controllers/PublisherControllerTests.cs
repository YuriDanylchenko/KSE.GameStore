using System.Net;
using System.Net.Http.Json;
using KSE.GameStore.ApplicationCore.Mapping;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.DataAccess;
using KSE.GameStore.Web.Mapping;
using KSE.GameStore.Web.Requests.Publishers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KSE.GameStore.Tests.IntegrationTests.Controllers;

public class PublisherControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PublisherControllerTests(WebApplicationFactory<Program> factory)
    {
        var dbName = $"PublisherTestDb-{Guid.NewGuid()}";
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
                services.AddDbContext<GameStoreDbContext>(options => { options.UseInMemoryDatabase(dbName); });

                services.AddAutoMapper(typeof(ApplicationCoreMappingProfile), typeof(WebMappingProfile));
            });
        });
    }

    [Fact]
    public async Task GetAllPublishers_ReturnsOkAndEmptyListInitially()
    {
        // Act
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/publishers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<PublisherDTO>>();
        Assert.NotNull(result);
        Assert.Empty(result!);
    }

    [Fact]
    public async Task CreatePublisher_ReturnsOkAndCreatedPublisher()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreatePublisherRequest("TestPub", "Test publisher", "https://test.com");


        // Act
        var response = await client.PostAsJsonAsync("/publishers", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PublisherDTO>();
        Assert.NotNull(result);
        Assert.Equal(request.Name, result!.Name);
        Assert.Equal(request.WebsiteUrl, result.WebsiteUrl);
        Assert.Equal(request.Description, result.Description);
    }

    [Fact]
    public async Task GetPublisherById_ReturnsOk_WhenExists()
    {
        // Arrange
        var client = _factory.CreateClient();
        var create = new CreatePublisherRequest("LookupPub", "desc", "https://lookup.com");

        var postResponse = await client.PostAsJsonAsync("/publishers", create);
        var created = await postResponse.Content.ReadFromJsonAsync<PublisherDTO>();

        // Act
        var getResponse = await client.GetAsync($"/publishers/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var result = await getResponse.Content.ReadFromJsonAsync<PublisherDTO>();
        Assert.Equal(created.Id, result!.Id);
        Assert.Equal("LookupPub", result.Name);
        Assert.Equal("https://lookup.com", result.WebsiteUrl);
        Assert.Equal("desc", result.Description);
    }

    [Fact]
    public async Task DeletePublisher_ReturnsNoContent_WhenExists()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreatePublisherRequest("ToDelete", "desc", "https://delete.com");

        var createdResponse = await client.PostAsJsonAsync("/publishers", request);
        var created = await createdResponse.Content.ReadFromJsonAsync<PublisherDTO>();

        // Act
        var deleteResponse = await client.DeleteAsync($"/publishers/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task UpdatePublisher_ReturnsOk_WhenValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = new CreatePublisherRequest(
            "Updatable",
            "before",
            "https://before.com"
        );


        var createResponse = await client.PostAsJsonAsync("/publishers", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PublisherDTO>();

        var updateRequest = new UpdatePublisherRequest(
            created!.Id,
            "UpdatedName",
            "https://after.com",
            "after update"
        );

        // Act
        var updateResponse = await client.PutAsJsonAsync("/publishers", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<PublisherDTO>();

        Assert.Equal("UpdatedName", updated!.Name);
        Assert.Equal("https://after.com", updated.WebsiteUrl);
        Assert.Equal("after update", updated.Description);
    }
}