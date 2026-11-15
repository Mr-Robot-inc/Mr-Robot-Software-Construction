using CargoHub.Api.Data;
using CargoHub.Api.Repositories;
using CargoHub.Api.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

DotNetEnv.Env.Load("../../.env");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = new NpgsqlConnectionStringBuilder(
    builder.Configuration.GetConnectionString("Default"))
{
    Password = Environment.GetEnvironmentVariable("DB_PASSWORD")
};

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString.ConnectionString));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
