using KSE.GameStore.ApplicationCore.Mapping;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.Web.Mapping;
using KSE.GameStore.Web.Requests.Platforms;
using KSE.GameStore.Web.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace KSE.GameStore.Tests.IntegrationTests.Controllers;

public class PlatformsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string jwtToken;
    private readonly string jwtKey = "G3BaSPlqaX3Bi6zGEvV8Q4PL5uSTZmDKJ8YjWq2C9ujd6lW5TXcMTaTzQkl3CVxe2nDgH6YdXcw";

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

                services.AddDbContext<GameStoreDbContext>(options => { options.UseInMemoryDatabase(dbName); });
                services.AddAutoMapper(typeof(ApplicationCoreMappingProfile), typeof(WebMappingProfile));
                services.AddScoped<IAuthService, AuthService>();

                services.Configure<JwtBearerOptions>(options => { options.TokenValidationParameters = AuthService.CreateTokenValidationParameters(jwtKey); });
            });

            builder.Configure(app =>
            {
                app.UseAuthorization();
                app.UseAuthentication();
            });
        });

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GameStoreDbContext>();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

        context.Database.EnsureCreated();

        var region = context.Regions.FirstOrDefault() ?? new Region { Name = "Default", Code = "DR" };
        if (region.Id == 0)
        {
            context.Regions.Add(region);
            context.SaveChanges();
        }

        var email = "test@test.com";
        var password = "Test123!";
        var userDto = authService.RegisterUserAsync(email, password, region.Id).GetAwaiter().GetResult()
            ?? throw new Exception("User registration failed.");

        var roles = new List<RoleDTO>
        {
            new(1, "Admin")
        };

        userDto = userDto with { Roles = roles };
        var loginDto = authService.LoginUserAsync(email, password).GetAwaiter().GetResult();
        var tokenResult = authService.GenerateUserJwtToken(loginDto!);
        jwtToken = tokenResult.Token;
        if (string.IsNullOrEmpty(jwtToken))
            throw new Exception("JWT token generation failed.");
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_Initially()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
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
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var newPlatform = new CreatePlatformRequest("PlayStation");

        var createResponse = await client.PostAsJsonAsync("/platforms", newPlatform);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var createdId = await createResponse.Content.ReadFromJsonAsync<int>();
        Assert.True(createdId > 0);

        var getResponse = await client.GetAsync($"/platforms/{createdId}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<PlatformResponse>();
        Assert.NotNull(fetched);
        Assert.Equal("PlayStation", fetched.Name);
    }

    [Fact]
    public async Task Update()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var platform = new CreatePlatformRequest("Wii");
        var createResponse = await client.PostAsJsonAsync("/platforms", platform);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var createdId = await createResponse.Content.ReadFromJsonAsync<int>();

        var updated = new UpdatePlatformRequest("Wii U");
        var updateResponse = await client.PutAsJsonAsync($"/platforms/{createdId}", updated);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var getResponse = await client.GetAsync($"/platforms/{createdId}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<PlatformResponse>();
        Assert.NotNull(fetched);
        Assert.Equal("Wii U", fetched.Name);
    }

    [Fact]
    public async Task Delete()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var platform = new CreatePlatformRequest("Stadia");
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
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var updated = new UpdatePlatformRequest("NonExistent");
        var response = await client.PutAsJsonAsync("/platforms/9999", updated);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NotFound_Returns404()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await client.DeleteAsync("/platforms/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}