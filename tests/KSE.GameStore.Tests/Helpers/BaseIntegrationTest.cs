using KSE.GameStore.ApplicationCore.Mapping;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using KSE.GameStore.Web.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KSE.GameStore.Tests.Helpers;

public class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;

    public BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string JwtKey = "G3BaSPlqaX3Bi6zGEvV8Q4PL5uSTZmDKJ8YjWq2C9ujd6lW5TXcMTaTzQkl3CVxe2nDgH6YdXcw";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTest");
        
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext configuration
            var dbContext = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<GameStoreDbContext>));
            if (dbContext != null)
                services.Remove(dbContext);

            // Add in-memory database with unique name per test
            var dbName = $"GameStoreTestDb-{Guid.NewGuid()}";
            services.AddDbContext<GameStoreDbContext>(options => 
            {
                options.UseInMemoryDatabase(dbName);
            });

            // Add AutoMapper
            services.AddAutoMapper(typeof(ApplicationCoreMappingProfile), typeof(WebMappingProfile));

            // Configure JWT authentication for tests
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = AuthService.CreateTokenValidationParameters(JwtKey);
            });

            services.AddAuthorization();

            // Add repositories
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IGameRepository, GameRepository>();

            // Register AuthService with the JWT key
            services.AddScoped<IAuthService>(provider =>
            {
                var userRepo = provider.GetRequiredService<IRepository<User, Guid>>();
                var roleRepo = provider.GetRequiredService<IRepository<Role, int>>();
                var regionRepo = provider.GetRequiredService<IRepository<Region, int>>();
                var refreshTokenRepo = provider.GetRequiredService<IRepository<RefreshToken, int>>();
                var mapper = provider.GetRequiredService<AutoMapper.IMapper>();
                
                return new AuthService(userRepo, roleRepo, regionRepo, refreshTokenRepo, mapper, JwtKey);
            });
        });
    }

    public async Task SeedTestDataAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreDbContext>();
        
        await dbContext.Database.EnsureCreatedAsync();

        // Seed basic test data if it doesn't exist
        if (!dbContext.Regions.Any())
        {
            dbContext.Regions.Add(new Region { Name = "Default", Code = "DR" });
            await dbContext.SaveChangesAsync();
        }

        if (!dbContext.Roles.Any())
        {
            dbContext.Roles.AddRange(
                new Role { Name = "Admin" },
                new Role { Name = "User" }
            );
            await dbContext.SaveChangesAsync();
        }
    }
}
