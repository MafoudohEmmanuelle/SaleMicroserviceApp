using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService.Data;
using UserService.Models;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["JwtKey"];
if (jwtKey is null || jwtKey.Length < 32)
    throw new Exception("JwtKey must be set in environment variables and at least 32 chars.");

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithOrigins("http://localhost:5173", "http://web:4173");
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=/app/data/users.db"));

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Better for Docker
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Seed an admin user by default if no users exist. Credentials are taken from env vars if provided.
    if (!db.Users.Any())
    {
        var adminUsername = builder.Configuration["AdminUsername"] ?? "admin";
        var adminPassword = builder.Configuration["AdminPassword"] ?? "admin123";
        var admin = new User
        {
            Username = adminUsername,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            Role = UserRole.Admin
        };
        db.Users.Add(admin);
        db.SaveChanges();
        Console.WriteLine($"Seeded admin user: {adminUsername}");
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("DevCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
