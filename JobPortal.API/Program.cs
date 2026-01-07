
using JobPortal.API.Data;
using JobPortal.API.Application.Jobs;
using JobPortal.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Xml;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var conn = builder.Configuration.GetConnectionString("SqlServer");

//DbContext (EF Core)
builder.Services.AddDbContext<AppDbContext>(o=>o.UseSqlServer(conn));

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

//Register repository & service for Jobs
builder.Services.AddScoped<IJobRepository,EfJobRepository>();
builder.Services.AddScoped<IJobService,JobService>();

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

// endpoint of Jobs
app.MapGet("/api/jobs", async (string? q, string? location, IJobRepository repo, CancellationToken ct) =>
{
   var jobs = await repo.ListActiveAsync(q, location,ct);
   return Results.Ok(jobs.Select(j=> new {j.Id,j.Title,j.Description,j.Location,j.Salary,CompanyProfile = new {j.Company.CompanyName,j.Company.Email},
   j.PostedUtc,j.SkillsCsv,j.IsActive}));
});

app.MapPost("/api/jobs", async(JobCreateDto dto, IJobService service, CancellationToken ct)=>
{
    // TEMP : open in Step 2; will be protected by Company role in Step3
    var job = await service.CreateJobAsync(dto.CompanyEmail,dto.Title, dto.Description, dto.Location, dto.Salary,dto.Skills,ct);
    return Results.Created($"/api/jobs/{job.Id}",job);
});
app.Run();

//Contracts
// OOP + DI example contract
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

//DTO
public record JobCreateDto(string CompanyEmail, string Title, string Description, string Location, decimal Salary, List<string>?Skills);
