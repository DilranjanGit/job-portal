
//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var conn = builder.Configuration.GetConnectionString("SqlServer");

// CORS for Angular dev server
builder.Services.AddCors(o => o.AddPolicy("ng",
    p => p.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200")));

// Health Checks: basic + SQL Server
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API is up"))
    .AddSqlServer(conn!, name: "sql", tags: new[] { "db", "sqlserver" });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Example DI: register a simple service to demonstrate DI
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("ng");

// Health endpoint
app.MapHealthChecks("/health");

// A simple endpoint to show DI working & basic OOP (interface → implementation)
app.MapGet("/api/ping", (IDateTimeProvider clock) =>
{
    return Results.Ok(new { message = "pong", serverTimeUtc = clock.UtcNow });
});

app.Run();

// OOP + DI example contract
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
