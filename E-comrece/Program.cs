using DotNetEnv;
using E_comerce.Data;
using E_comerce.Services;
using E_commerce.Services;
using E_comrece.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Net.Http;
using System.Text;
using ProductService = E_commerce.Services.ProductService;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env
Env.Load();

// Registering DbContext for PostgreSQL
// Load DB password from .env
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")
                 ?? throw new InvalidOperationException("DB_PASSWORD not set in .env");

// Build connection string manually using env
var connectionString = $"Host=localhost;Port=5432;Database=E_comerce;Username=postgres;Password={dbPassword}";

// Registering DbContext for PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 1;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// JWT Configuration from .env
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET");
var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("JWT_SECRET not found in environment variables"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully.");
            return Task.CompletedTask;
        }
    };
});

// Configure CORS for Next.js
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<StoreService>();
builder.Services.AddScoped<BillboardService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<SizeService>();
builder.Services.AddScoped<ColorService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IImageStorageService, ImageStorageService>();
builder.Services.AddScoped<OrderService>();

// Stripe & HttpClient
builder.Services.AddHttpClient();

// Memory Cache
builder.Services.AddMemoryCache();

var app = builder.Build();

// Enable CORS
app.UseCors("AllowNextJs");

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Stripe configuration from .env
var stripeKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY")
                ?? throw new InvalidOperationException("STRIPE_SECRET_KEY not found in environment variables");
StripeConfiguration.ApiKey = stripeKey;

app.Run();
