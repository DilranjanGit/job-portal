using JobPortal.API.Data;
using JobPortal.API.Application.Jobs;
using JobPortal.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JobPortal.API.Application.Students;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var conn = builder.Configuration.GetConnectionString("SqlServer");

//DbContext (EF Core)
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(conn));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "DksSecretKey123432Tempdefaultkey";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "JobPortal";
var audience = builder.Configuration["Jwt:Audience"] ?? "JobPortalClients";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCors(o => o.AddPolicy("ng",
    p => p.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200")));

builder.Services.AddHealthChecks()
    .AddSqlServer(conn!, name: "sql", tags: new[] { "db", "sqlserver" });

builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddScoped<IJobRepository, EfJobRepository>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "JobPortal.API v1", Version = "v1" });

    // THIS IS THE MISSING CODE BLOCK
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});


try
{
    var app = builder.Build();

    app.UseMiddleware<ErrorHandlingMiddleware>();

    // Seed roles and admin user using AppRoles class
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        string[] roles = new[] { AppRoles.Student, AppRoles.Company, AppRoles.Admin };
        foreach (var role in roles)
        {
            if (!roleManager.RoleExistsAsync(role).Result)
            {
                var created = roleManager.CreateAsync(new IdentityRole(role)).Result;
                if (!created.Succeeded)
                {
                    throw new Exception($"Failed to create role: {role} :" + string.Join(", ", created.Errors.Select(e => e.Description)));
                }
            }
        }
        var adminEmail = builder.Configuration["AdminUser:Email"] ?? "admin@jobportal.com";
        var adminPassword = builder.Configuration["AdminUser:Password"] ?? "Admin@123";
        var adminUser = userManager.FindByEmailAsync(adminEmail).Result;
        if (adminUser is null)
        {
            adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
            var result = userManager.CreateAsync(adminUser, adminPassword).Result;
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create admin user: {adminEmail} :" + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            userManager.AddToRoleAsync(adminUser, AppRoles.Admin).Wait();
        }
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors("ng");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapHealthChecks("/health");
    // If you get an error here, ensure AddControllers() is called and you have at least one controller in your project.
    app.MapControllers();

    // Job creation endpoint
    app.MapPost("/api/jobs", async(JobPortal.API.DTOs.JobCreateDto dto, IJobService service, HttpContext ctx, CancellationToken ct) =>
    {
        if (!ctx.User.IsInRole(JobPortal.API.Domain.AppRoles.Company)) return Results.Forbid();
        var email = ctx.User.FindFirst("email")?.Value!;
        var job = await service.CreateJobAsync(dto.CompanyEmail, dto.Title, dto.Description, dto.Location, dto.Salary, dto.Skills, ct);
        return Results.Created($"/api/jobs/{job.Id}", job);
    }).RequireAuthorization();

    // Company registration endpoint
    app.MapPost("/api/auth/register/company", async(JobPortal.API.DTOs.RegisterCompanyDto dto, UserManager<ApplicationUser> um, AppDbContext db) =>
    {
        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
        var result = await um.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return Results.BadRequest(result.Errors);
        await um.AddToRoleAsync(user, JobPortal.API.Domain.AppRoles.Company);
        db.Companies.Add(new CompanyProfile { CompanyName = dto.CompanyName, Email = dto.Email, Location = dto.Location });
        await db.SaveChangesAsync();
        return Results.Ok();
    });

    // Login endpoint
    app.MapPost("/api/auth/login", async(JobPortal.API.DTOs.LoginDto dto, UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm) =>
    {
        var user = await um.FindByEmailAsync(dto.Email);
        if (user is null) return Results.BadRequest("Invalid email");
        var result = await sm.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded) return Results.BadRequest("Invalid password");
        var roles = await um.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? JobPortal.API.Domain.AppRoles.Student;
        var jwtKey = builder.Configuration["Jwt:Key"] ?? "DksSecretKey123432Tempdefaultkey";
        var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "JobPortal";
        var audience = builder.Configuration["Jwt:Audience"] ?? "JobPortalClients";
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<System.Security.Claims.Claim>
        {
            new ("sub", user.Id),
            new ("email", user.Email!),
            new ("role", role)
        };
        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            jwtIssuer, audience, claims, expires: DateTime.Now.AddHours(8), signingCredentials: creds);
        var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        return Results.Ok(new { token = jwt, role = role });
    }).AllowAnonymous();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Unhandled exception in Program.cs: {ex.Message}\n{ex.StackTrace}");
    throw;
}

// Contracts
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
