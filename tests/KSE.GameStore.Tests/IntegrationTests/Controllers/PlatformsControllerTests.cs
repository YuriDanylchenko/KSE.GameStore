using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.Tests.Helpers;
using KSE.GameStore.Web.Requests.Platforms;
using KSE.GameStore.Web.Responses;
using System.Net;
using System.Net.Http.Json;

namespace KSE.GameStore.Tests.IntegrationTests.Controllers;

public class PlatformsControllerTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAll_ReturnsEmptyList_Initially()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var response = await Client.GetAsync("/platforms");
        response.EnsureSuccessStatusCode();

        var platforms = await response.Content.ReadFromJsonAsync<List<PlatformDTO>>();
        Assert.NotNull(platforms);
        Assert.Empty(platforms!);
    }

    [Fact]
    public async Task Create_And_GetById()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var newPlatform = new CreatePlatformRequest("PlayStation");

        var createResponse = await Client.PostAsJsonAsync("/platforms", newPlatform);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var createdId = await createResponse.Content.ReadFromJsonAsync<int>();
        Assert.True(createdId > 0);

        var getResponse = await Client.GetAsync($"/platforms/{createdId}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<PlatformResponse>();
        Assert.NotNull(fetched);
        Assert.Equal("PlayStation", fetched.Name);
    }

    [Fact]
    public async Task Update()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var platform = new CreatePlatformRequest("Xbox");
        var createResponse = await Client.PostAsJsonAsync("/platforms", platform);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var createdId = await createResponse.Content.ReadFromJsonAsync<int>();

        var updated = new UpdatePlatformRequest("Xbox Series X");
        var updateResponse = await Client.PutAsJsonAsync($"/platforms/{createdId}", updated);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var getResponse = await Client.GetAsync($"/platforms/{createdId}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<PlatformResponse>();
        Assert.Equal("Xbox Series X", fetched!.Name);
    }

    [Fact]
    public async Task Delete()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var platform = new CreatePlatformRequest("Stadia");
        var createResponse = await Client.PostAsJsonAsync("/platforms", platform);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var createdId = await createResponse.Content.ReadFromJsonAsync<int>();

        var deleteResponse = await Client.DeleteAsync($"/platforms/{createdId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var getResponse = await Client.GetAsync($"/platforms/{createdId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Update_NotFound_Returns404()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var updated = new UpdatePlatformRequest("NonExistent");
        var response = await Client.PutAsJsonAsync("/platforms/9999", updated);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NotFound_Returns404()
    {
        TestHelper.SetupAuthenticatedClient(Client, "Admin");
        var response = await Client.DeleteAsync("/platforms/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
