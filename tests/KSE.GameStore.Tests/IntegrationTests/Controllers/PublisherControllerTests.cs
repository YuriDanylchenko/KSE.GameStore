using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.Tests.Helpers;
using KSE.GameStore.Web.Requests.Publishers;
using System.Net;
using System.Net.Http.Json;

namespace KSE.GameStore.Tests.IntegrationTests.Controllers;

public class PublisherControllerTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllPublishers_ReturnsOkAndEmptyListInitially()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var response = await Client.GetAsync("/publishers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var publishers = await response.Content.ReadFromJsonAsync<List<PublisherDTO>>();
        Assert.NotNull(publishers);
        Assert.Empty(publishers);
    }

    [Fact]
    public async Task CreatePublisher_ReturnsOkAndCreatedPublisher()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var request = new CreatePublisherRequest("TestPub", "Test publisher", "https://test.com");

        var response = await Client.PostAsJsonAsync("/publishers", request);

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
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var create = new CreatePublisherRequest("LookupPub", "desc", "https://lookup.com");

        var postResponse = await Client.PostAsJsonAsync("/publishers", create);
        var created = await postResponse.Content.ReadFromJsonAsync<PublisherDTO>();

        var getResponse = await Client.GetAsync($"/publishers/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var result = await getResponse.Content.ReadFromJsonAsync<PublisherDTO>();
        Assert.Equal(created.Id, result!.Id);
        Assert.Equal("LookupPub", result.Name);
        Assert.Equal("https://lookup.com", result.WebsiteUrl);
        Assert.Equal("desc", result.Description);
    }

    [Fact]
    public async Task DeletePublisher_ReturnsOk_WhenExists()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var createRequest = new CreatePublisherRequest("DeleteMe", "To be deleted", "https://delete.com");

        var createResponse = await Client.PostAsJsonAsync("/publishers", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PublisherDTO>();

        var deleteResponse = await Client.DeleteAsync($"/publishers/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var getResponse = await Client.GetAsync($"/publishers/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task UpdatePublisher_ReturnsOk_WhenValid()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var createRequest = new CreatePublisherRequest(
            "Updatable",
            "before",
            "https://before.com"
        );

        var createResponse = await Client.PostAsJsonAsync("/publishers", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PublisherDTO>();

        var updateRequest = new UpdatePublisherRequest(
            created!.Id,
            "UpdatedName",
            "https://after.com",
            "after update"
        );

        var updateResponse = await Client.PutAsJsonAsync("/publishers", updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<PublisherDTO>();

        Assert.Equal("UpdatedName", updated!.Name);
        Assert.Equal("https://after.com", updated.WebsiteUrl);
        Assert.Equal("after update", updated.Description);
    }
}
