using System.Security.Claims;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace KSE.GameStore.Tests.Helpers;

public static class TestHelper
{
    /// <summary>
    /// Creates a JWT token for testing with Admin role
    /// </summary>
    public static string CreateTestUserToken()
    {
        var options = new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase("TestTokenDb").Options;
        
        using var dbContext = new GameStoreDbContext(options);
        
        // Create test region if it doesn't exist
        var region = dbContext.Regions.FirstOrDefault() ?? new Region { Name = "Test Region", Code = "TR" };
        if (region.Id == 0)
        {
            dbContext.Regions.Add(region);
            dbContext.SaveChanges();
        }

        // Create test user DTO for token generation
        var testUser = new UserDTO(
            Id: Guid.NewGuid(),
            Email: "test@gamestore.com",
            HashedPassword: string.Empty,
            PasswordSalt: string.Empty,
            Region: new RegionDTO(region.Id, region.Name, region.Code),
            Roles: new List<RoleDTO> { new(1, "Admin"), new(2, "User") }
        );

        // Create AuthService instance for token generation
        var jwtKey = "G3BaSPlqaX3Bi6zGEvV8Q4PL5uSTZmDKJ8YjWq2C9ujd6lW5TXcMTaTzQkl3CVxe2nDgH6YdXcw";
        var authService = new AuthService(
            null!, null!, null!, null!, null!, jwtKey, 30
        );

        var tokenResult = authService.GenerateUserJwtToken(testUser);
        return tokenResult.Token;
    }

    /// <summary>
    /// Creates a JWT token for testing with specific roles
    /// </summary>
    public static string CreateTestUserTokenWithRoles(params string[] roles)
    {
        var options = new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase("TestTokenDb").Options;
        
        using var dbContext = new GameStoreDbContext(options);
        
        // Create test region if it doesn't exist
        var region = dbContext.Regions.FirstOrDefault() ?? new Region { Name = "Test Region", Code = "TR" };
        if (region.Id == 0)
        {
            dbContext.Regions.Add(region);
            dbContext.SaveChanges();
        }

        var roleDTOs = roles.Select((role, index) => new RoleDTO(index + 1, role)).ToList();

        // Create test user DTO for token generation
        var testUser = new UserDTO(
            Id: Guid.NewGuid(),
            Email: "test@gamestore.com",
            HashedPassword: string.Empty,
            PasswordSalt: string.Empty,
            Region: new RegionDTO(region.Id, region.Name, region.Code),
            Roles: roleDTOs
        );

        // Create AuthService instance for token generation
        var jwtKey = "G3BaSPlqaX3Bi6zGEvV8Q4PL5uSTZmDKJ8YjWq2C9ujd6lW5TXcMTaTzQkl3CVxe2nDgH6YdXcw";
        var authService = new AuthService(
            null!, null!, null!, null!, null!, jwtKey, 30
        );

        var tokenResult = authService.GenerateUserJwtToken(testUser);
        return tokenResult.Token;
    }

    /// <summary>
    /// Creates an HTTP request with authentication header
    /// </summary>
    public static HttpRequestMessage CreateAuthenticatedRequest(HttpMethod method, string uri, object? content = null)
    {
        var token = CreateTestUserToken();
        var request = new HttpRequestMessage(method, uri);
        
        if (content != null)
        {
            request.Content = JsonContent.Create(content);
        }
        
        request.Headers.Add("Authorization", $"Bearer {token}");
        return request;
    }

    /// <summary>
    /// Creates an HTTP client with default authentication header
    /// </summary>
    public static void SetupAuthenticatedClient(HttpClient client, params string[] roles)
    {
        var token = roles.Length > 0 ? CreateTestUserTokenWithRoles(roles) : CreateTestUserToken();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Creates test entities and returns their IDs
    /// </summary>
    public static async Task<(int PublisherId, int GenreId, int PlatformId)> CreateTestEntitiesAsync(GameStoreDbContext dbContext)
    {
        var publisher = new Publisher { Name = "Test Publisher" };
        var genre = new Genre { Name = "Test Genre" };
        var platform = new Platform { Name = "Test Platform" };
        
        dbContext.Publishers.Add(publisher);
        dbContext.Genres.Add(genre);
        dbContext.Platforms.Add(platform);
        await dbContext.SaveChangesAsync();

        return (publisher.Id, genre.Id, platform.Id);
    }
}
