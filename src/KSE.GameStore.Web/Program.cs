using KSE.GameStore.ApplicationCore.Interfaces;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.DataAccess;
using KSE.GameStore.DataAccess.Repositories;
using KSE.GameStore.Web.Infrastructure;
using KSE.GameStore.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (!builder.Environment.IsEnvironment("IntegrationTest"))
{
    builder.Services.AddDbContext<GameStoreDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("GameStoreDb"),
            x => x.MigrationsAssembly("KSE.GameStore.Migrations")));
}

builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped<IPlatformsService, PlatformsService>();
builder.Services.AddControllers();

builder.Services.AddScoped<IGenreService, GenreService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();

public partial class Program;