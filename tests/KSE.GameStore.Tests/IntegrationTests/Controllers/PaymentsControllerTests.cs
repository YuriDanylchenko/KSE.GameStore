using System.Net;
using System.Net.Http.Json;
using KSE.GameStore.ApplicationCore.Mapping;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.Web.Mapping;
using KSE.GameStore.Web.Requests.Payments;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KSE.GameStore.Tests.IntegrationTests.Controllers;

public class PaymentsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PaymentsControllerTests(WebApplicationFactory<Program> factory)
    {
        var dbName = $"PaymentsTestDb-{Guid.NewGuid()}";
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

    public sealed class TestDataHelper
    {
        private readonly WebApplicationFactory<Program> _factory;

        public TestDataHelper(WebApplicationFactory<Program> factory) => _factory = factory;

        public async Task<(Order order, GameStoreDbContext ctx)> CreateTestEntitiesAsync()
        {
            var scope = _factory.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<GameStoreDbContext>();

            // 1) Game dependencies
            var (publisher, genre, platform) = await CreateDependenciesAsync(ctx);

            // 2) Game and Price
            var game = await CreateGameAsync(ctx, publisher, genre, platform);

            // 3) User → Order → OrderItem
            var (region, _, order, orderItem) = await CreateUserFlowAsync(ctx, game);

            return (order, ctx);
        }
        
        private static async Task<Game> CreateGameAsync(
            GameStoreDbContext ctx,
            Publisher publisher,
            Genre genre,
            Platform platform)
        {
            var game = new Game
            {
                CreatedAt   = DateTime.UtcNow,
                UpdatedAt   = DateTime.UtcNow,
                Description = "Test game description",
                Genres      = [genre],
                Platforms   = [platform],
                Publisher   = publisher,
                Title       = "Test Game",
                Prices      = []
            };

            var price = new GamePrice
            {
                StartDate = DateTime.UtcNow,
                Stock     = 10_000,
                Value     = 1_000,
                Game      = game
            };
            game.Prices.Add(price);

            ctx.Games.Add(game);
            ctx.Prices.Add(price);
            await ctx.SaveChangesAsync();

            return game;
        }
        
        private static async Task<(Publisher, Genre, Platform)>
            CreateDependenciesAsync(GameStoreDbContext ctx)
        {
            var publisher = new Publisher { Name = "Test Publisher" };
            var genre     = new Genre     { Name = "Test Genre"     };
            var platform  = new Platform  { Name = "Test Platform"  };

            ctx.AddRange(publisher, genre, platform);
            await ctx.SaveChangesAsync();

            return (publisher, genre, platform);
        }
        
        private static async Task<(
                Region Region,
                User   User,
                Order  Order,
                OrderItem OrderItem)>
            CreateUserFlowAsync(GameStoreDbContext ctx, Game game)
        {
            var region = new Region { Code = "EU", Name = "Europe", Users = [] };

            var user = new User
            {
                Email     = "test_email_user@test.com",
                Name      = "Test User",
                Role      = "Admin",
                Region    = region,
                Orders    = [],
                GameStock = []
            };
            region.Users.Add(user);

            var order = new Order
            {
                CreatedAt   = DateTime.UtcNow,
                UpdatedAt   = DateTime.UtcNow,
                Status      = OrderStatus.Initiated,
                User        = user,
                OrderItems  = []
            };
            user.Orders.Add(order);

            var orderItem = new OrderItem
            {
                Game     = game,
                Quantity = 1,
                Price    = game.Prices.First().Value,
                Order    = order
            };
            order.OrderItems.Add(orderItem);

            ctx.AddRange(region, user, order);
            await ctx.SaveChangesAsync();

            return (region, user, order, orderItem);
        }
    }
    
    #region GET /payments/{id}

    [Fact]
    public async Task GetPaymentById_ReturnsDto()
    {
        // Arrange
        var (order, ctx) = await new TestDataHelper(_factory).CreateTestEntitiesAsync();
        var client = _factory.CreateClient();
        var createPaymentRequest = new CreatePaymentRequest(order.Id, PaymentMethodRequest.CreditCards);

        await client.PostAsJsonAsync("/payments", createPaymentRequest);
        
        // Reload Order enity
        await ctx.Entry(order).ReloadAsync();
        await ctx.Entry(order).Collection(o => o.OrderItems).LoadAsync();
        await ctx.Entry(order).Reference(o => o.Payment).LoadAsync();
        var payment = order.Payment;

        // Act
        var response = await client.GetAsync($"/payments/{payment!.Id}");
        var dto = await response.Content.ReadFromJsonAsync<PaymentDTO>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(dto);
        Assert.Equal(payment.Id, dto!.Id);
        Assert.Equal(payment.OrderId, dto.OrderId);
    }
    
    [Fact]
    public async Task GetById_ReturnsNotFound_ForInvalidId()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/payments/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion
    
    #region GET /payments
    
    [Fact]
    public async Task GetAllPayments_ReturnsList()
    {
        // Arrange
        var (order, ctx) = await new TestDataHelper(_factory).CreateTestEntitiesAsync();
        var client       = _factory.CreateClient();

        var req = new CreatePaymentRequest(order.Id, PaymentMethodRequest.CreditCards);
        await client.PostAsJsonAsync("/payments", req);

        // Add the second order
        var (order2, _) = await new TestDataHelper(_factory).CreateTestEntitiesAsync();
        await client.PostAsJsonAsync(
            "/payments",
            new CreatePaymentRequest(order2.Id, PaymentMethodRequest.IBAN));

        // Act
        var response = await client.GetAsync("/payments?pageNumber=1&pageSize=20");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var list = await response.Content.ReadFromJsonAsync<List<PaymentDTO>>();
        Assert.NotNull(list);
        Assert.Equal(2, list!.Count);
    }

    [Fact]
    public async Task GetAllPayments_InvalidPaging_ReturnsBadRequest()
    {
        var client   = _factory.CreateClient();
        var response = await client.GetAsync("/payments?pageNumber=0&pageSize=10");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    #endregion
    
    #region POST /payments

    [Fact]
    public async Task CreatePayment_ReturnsExcelAndUpdatesOrder()
    {
        // Arrange
        var (order, ctx) = await new TestDataHelper(_factory).CreateTestEntitiesAsync();
        var client = _factory.CreateClient();

        var createReq = new CreatePaymentRequest(order.Id, PaymentMethodRequest.CreditCards);

        // Act
        var response = await client.PostAsJsonAsync("/payments", createReq);
        var bytes = await response.Content.ReadAsByteArrayAsync();
        
        await ctx.Entry(order).ReloadAsync();
        await ctx.Entry(order).Reference(o => o.Payment).LoadAsync();
        
        var licenses = await ctx.Stock
            .Where(s => s.UserId == order.UserId)
            .ToListAsync();

        // Assert : файл
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                     response.Content.Headers.ContentType!.MediaType);
        
        Assert.NotEmpty(bytes);

        // Assert: state of DB
        Assert.Equal(OrderStatus.Payed, order.Status);
        Assert.NotNull(order.Payment);
        Assert.True(order.Payment!.Confirmed);

        // We have only one game - so one license
        Assert.Single(licenses);
    }

    [Fact]
    public async Task CreatePayment_InvalidOrderId_ReturnsNotFound()
    {
        var client = _factory.CreateClient();

        var createReq = new CreatePaymentRequest(999_999, PaymentMethodRequest.IBAN);
        var response  = await client.PostAsJsonAsync("/payments", createReq);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region PUT /payments
    
    [Fact]
    public async Task UpdatePayment_ReturnsChangedDtoAndPersists()
    {
        // Arrange
        var (order, ctx) = await new TestDataHelper(_factory).CreateTestEntitiesAsync();
        var client       = _factory.CreateClient();
    
        await client.PostAsJsonAsync(
            "/payments",
            new CreatePaymentRequest(order.Id, PaymentMethodRequest.CreditCards));
    
        await ctx.Entry(order).Reference(o => o.Payment).LoadAsync();
        var payment = order.Payment!;
        
        var newDate = DateTime.UtcNow.AddMinutes(-1);
        var updateReq = new UpdatePaymentRequest(payment.Id, newDate, PaymentMethodRequest.IBAN);
    
        // Act
        var response = await client.PutAsJsonAsync("/payments", updateReq);
        var dto = await response.Content.ReadFromJsonAsync<PaymentDTO>();
    
        // Assert HTTP
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // Assert Dto
        Assert.Equal(newDate, dto!.PayedAt);
        Assert.Equal(PaymentMethodDTO.IBAN, dto.PaymentMethod);
    }
    
    [Fact]
    public async Task UpdatePayment_FutureDate_ReturnsBadRequest()
    {
        var (order, ctx) = await new TestDataHelper(_factory).CreateTestEntitiesAsync();
        var client       = _factory.CreateClient();
    
        await client.PostAsJsonAsync(
            "/payments",
            new CreatePaymentRequest(order.Id, PaymentMethodRequest.CreditCards));
    
        await ctx.Entry(order).Reference(o => o.Payment).LoadAsync();
        var payment = order.Payment!;
    
        var badReq = new UpdatePaymentRequest(
            payment.Id,
            DateTime.UtcNow.AddDays(2),          // у майбутньому
            PaymentMethodRequest.CreditCards);
    
        var response = await client.PutAsJsonAsync("/payments", badReq);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdatePayment_NotFound_Returns404()
    {
        var client = _factory.CreateClient();
    
        var updateReq = new UpdatePaymentRequest(
            Id: 999_999,
            PayedAt: DateTime.UtcNow,
            PaymentMethod: PaymentMethodRequest.IBAN);
    
        var response = await client.PutAsJsonAsync("/payments", updateReq);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    #endregion
}