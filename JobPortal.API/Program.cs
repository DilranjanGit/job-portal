using JobPortal.API.Data;
using JobPortal.API.Application.Jobs;
using JobPortal.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Xml;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Identity.Client.Extensibility;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var conn = builder.Configuration.GetConnectionString("SqlServer");

//DbContext (EF Core)
builder.Services.AddDbContext<AppDbContext>(o=>o.UseSqlServer(conn));

// Identity
builder.Services.AddIdentity<ApplicationUser,IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"]?? "DksSecretKey123432Tempdefaultkey"; // TEMP default key for dev
var jwtIssuer = builder.Configuration["Jwt:Issuer"]?? "JobPortal";
var audience = builder.Configuration["Jwt:Audience"]?? "JobPortalClients";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options=>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime =  true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
builder.Services.AddAuthorization();
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

// Seed roles "Student" and "Company" and an admin user
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Create roles if not exists
    string[] roles = new[] { AppRoles.Student, AppRoles.Company, AppRoles.Admin };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
           var created = await roleManager.CreateAsync(new IdentityRole(role));
            if(!created.Succeeded)
            {
                throw new Exception($"Failed to create role: {role} :"+ string.Join(", ", created.Errors.Select(e=>e.Description)));
            }
        }
    }

    // Create a default admin user if not exists
    var adminEmail = builder.Configuration["AdminUser:Email"] ?? "admin@jobportal.com";
    var adminPassword = builder.Configuration["AdminUser:Password"] ?? "Admin@123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser is null)
    {
        adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (!result.Succeeded)
        {
            throw new Exception($"Failed to create admin user: {adminEmail} :" + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        //add admin role to the user
        await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
        
    }       
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("ng");

//add authentication & authorization middlewares
app.UseAuthentication();
app.UseAuthorization();


// Health endpoint
app.MapHealthChecks("/health");

// A simple endpoint to show DI working & basic OOP (interface → implementation)
app.MapGet("/api/ping", (IDateTimeProvider clock) =>
{
    return Results.Ok(new { message = "pong", serverTimeUtc = clock.UtcNow });
});

// endpoint of Jobs
/*
app.MapGet("/api/jobs", async (JobCreateDto dto, IJobService service,  CancellationToken ct) =>
{
    

   var job = await service.CreateJobAsync(email,dto.Title,dto.Description ,dto.Location,dto.Salary,dto.Skills,ct);
   return Results.Created($"/api/jobs/{job.Id}",job);
    
}).RequireAuthorization();
*/

app.MapPost("/api/jobs", async(JobCreateDto dto, IJobService service,HttpContext ctx, CancellationToken ct)=>
{
    // TEMP : open in Step 2; will be protected by Company role in Step3
    if(!ctx.User.IsInRole(AppRoles.Company)) return Results.Forbid();
    var email= ctx.User.FindFirst("email")?.Value!;

    var job = await service.CreateJobAsync(dto.CompanyEmail,dto.Title, dto.Description, dto.Location, dto.Salary,dto.Skills,ct);
    return Results.Created($"/api/jobs/{job.Id}",job);
}).RequireAuthorization();

//add register endpoints for students and companies 
app.MapPost("/api/auth/register/student", async (RegisterStudentDto dto, UserManager<ApplicationUser> um, AppDbContext db)=>
{
    var user = new ApplicationUser{ UserName = dto.Email, Email=dto.Email};
    var result = await um.CreateAsync(user,dto.Password);
    if(!result.Succeeded) return

    Results.BadRequest(result.Errors);
    await um.AddToRoleAsync(user, AppRoles.Student);
    db.Students.Add(new StudentProfile{FullName=dto.FullName,Email=dto.Email, PhoneNumber= dto.PhoneNumber,Education=dto.Education});
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPost("/api/auth/register/company",async(RegisterCompanyDto dto, UserManager<ApplicationUser> um, AppDbContext db)=>
{
   var user = new ApplicationUser{UserName=dto.Email,Email=dto.Email};
   var result = await um.CreateAsync(user,dto.Password);
   if(!result.Succeeded) return Results.BadRequest(result.Errors);
   await um.AddToRoleAsync(user, AppRoles.Company);     
   db.Companies.Add(new CompanyProfile{CompanyName=dto.CompanyName,Email=dto.Email,Location=dto.Location});
   await db.SaveChangesAsync();
   return Results.Ok();
});

app.MapPost("/api/auth/login",async(LoginDto dto, UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm)=>
{
    var user = await um.FindByEmailAsync(dto.Email);
    if(user is null) return Results.BadRequest("Invalid email");
    var result = await sm.CheckPasswordSignInAsync(user,dto.Password,false);
    if(!result.Succeeded) return Results.BadRequest("Invalid password");
   
   var roles = await um.GetRolesAsync(user);
   var role = roles.FirstOrDefault() ?? AppRoles.Student; // Default to Student if no role found

   var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
   var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
   var claims = new List<System.Security.Claims.Claim>
    {
        new ("sub", user.Id),
        new ("email", user.Email!),
        new ("role", role)
    };
    var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
        jwtIssuer,audience,claims,expires: DateTime.Now.AddHours(8),signingCredentials: creds);

        var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(new {token=jwt, role=role});

    // In real app, generate JWT token here
    //return Results.Ok(new {message="Login successful"});
}).AllowAnonymous();
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


public record RegisterStudentDto(string Email, string Password, string FullName, string Education,string PhoneNumber);
public record RegisterCompanyDto(string Email, string Password, string CompanyName, string Location);
public record LoginDto(string Email, string Password);
