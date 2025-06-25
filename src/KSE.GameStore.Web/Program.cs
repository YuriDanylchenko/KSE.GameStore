using KSE.GameStore.ApplicationCore.Infrastructure;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.ApplicationCore.Mapping;
using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Repositories;
using KSE.GameStore.Web.Mapping;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------
// Logging
// ---------------------------------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ---------------------------------------------
// Core services
// ---------------------------------------------
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddSwaggerGen();

if (!builder.Environment.IsEnvironment("IntegrationTest"))
{
    builder.Services.AddDbContext<GameStoreDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("GameStoreDb"),
            x => x.MigrationsAssembly("KSE.GameStore.Migrations")));
}

// Repositories
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

// Domain services
builder.Services.AddScoped<IPlatformsService, PlatformsService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<ICartService, CartService>();

// AutoMapper
builder.Services.AddAutoMapper(cfg => { cfg.AllowNullCollections = true; },
    typeof(ApplicationCoreMappingProfile),
    typeof(WebMappingProfile));

// MVC controllers
builder.Services.AddControllers();

// ---------------------------------------------
// Build pipeline
// ---------------------------------------------
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggerMiddleware>();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

public partial class Program;