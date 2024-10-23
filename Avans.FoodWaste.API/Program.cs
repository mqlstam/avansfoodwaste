using Avans.FoodWaste.API;
using Avans.FoodWaste.API.Middleware;
using Avans.FoodWaste.Application.Interfaces;
using Avans.FoodWaste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Avans.FoodWaste.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Register DbContexts
builder.Services.AddDbContext<FoodWasteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FoodWasteDatabase")));

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDatabase")));


// Register the DataSeeder as a hosted service
builder.Services.AddHostedService<DataSeeder>();


// Register application services
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

// Add Identity services
builder.Services.AddIdentity<IdentityUser<int>, IdentityRole<int>>(options =>
{
    // Configure Identity options here (password strength, etc.)  Example:
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false; // Adjust based on requirements
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;

})
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

// Add JWT Authentication



// Existing authentication setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Configure token validation parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = false, // Set to true in production
            ValidateAudience = false, // Set to true in production
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Optional: Adjust as needed
        };
        
        // Add event handlers
        options.Events = new JwtBearerEvents
        {
            // Handle authentication failures
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                
                var result = JsonSerializer.Serialize(new { message = "Authentication failed." });
                return context.Response.WriteAsync(result);
            },
            
            // Handle challenges (e.g., missing or invalid tokens)
            OnChallenge = context =>
            {
                // Skip the default logic
                context.HandleResponse();
                
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                
                var result = JsonSerializer.Serialize(new { message = "Unauthorized. Token is missing or invalid." });
                return context.Response.WriteAsync(result);
            }
        };
    });
builder.Services.AddTransient<SignInManager<IdentityUser<int>>>();
builder.Services.AddTransient<UserManager<IdentityUser<int>>>();

// Add authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Exception handler middleware MUST be before UseRouting
// app.UseMiddleware<ExceptionHandlerMiddleware>(); 
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();