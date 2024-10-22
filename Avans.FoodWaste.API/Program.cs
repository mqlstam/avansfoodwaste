using System.Text;
using Avans.FoodWaste.API;
using Avans.FoodWaste.API.Middleware;
using Avans.FoodWaste.Application.Interfaces;
using Avans.FoodWaste.Infrastructure.Data; 
using Microsoft.EntityFrameworkCore; 
using Avans.FoodWaste.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

// Register DbContext with the connection string from configuration
builder.Services.AddDbContext<FoodWasteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FoodWasteDatabase")));

// Register the DataSeeder as a hosted service
builder.Services.AddHostedService<DataSeeder>();

// Register application services
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = false, // Set to true in production
            ValidateAudience = false, // Set to true in production
            ValidateLifetime = true
        };
    });

// Add authorization
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// Use the ExceptionHandlerMiddleware
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
