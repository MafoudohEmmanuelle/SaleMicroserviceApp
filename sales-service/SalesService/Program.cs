using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SalesService.Data;
using SalesService.Clients;

var builder = WebApplication.CreateBuilder(args);

// ✅ ItemsServiceUrl must point to ProductsService (e.g. http://localhost:5036/)
var itemsServiceUrl = builder.Configuration["ItemsServiceUrl"] ?? "http://localhost:5036/";
var jwtKey = builder.Configuration["JwtKey"];
if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
    throw new Exception("JwtKey must be set and at least 32 characters");

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=/app/data/sales.db"));

// Register ProductClient with correct base addressv
builder.Services.AddHttpClient<ProductClient>(client =>
{
    client.BaseAddress = new Uri(itemsServiceUrl);
});

// Global CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithOrigins("http://localhost:5173", "http://web:4173"); // React dev server and docker web
    });
});

// JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(jwtKey);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            NameClaimType = System.Security.Claims.ClaimTypes.Name
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Console.WriteLine($"SalesService running. ItemsServiceUrl = {itemsServiceUrl}");

app.Run();