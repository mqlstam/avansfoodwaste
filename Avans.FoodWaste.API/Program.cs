using Avans.FoodWaste.API;
using Avans.FoodWaste.API.Middleware;
using Avans.FoodWaste.Application.Interfaces;
using Avans.FoodWaste.Infrastructure.Data; 
using Microsoft.EntityFrameworkCore; 
using Avans.FoodWaste.Application.Services;
using Avans.FoodWaste.Application.Interfaces; 
using Avans.FoodWaste.Application.Services;  

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FoodWasteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FoodWasteDatabase")));

// Register the DataSeeder as a hosted service
builder.Services.AddHostedService<DataSeeder>();

// PackageService registration
builder.Services.AddScoped<IPackageService, PackageService>();

builder.Services.AddScoped<IStudentService, StudentService>(); 

builder.Services.AddScoped<IReservationService, ReservationService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting(); // Middleware must be registered after UseRouting

// Use the ExceptionHandlerMiddleware
app.UseMiddleware<ExceptionHandlerMiddleware>(); // Add this line

app.UseAuthorization();

app.UseEndpoints(endpoints => 
{
    endpoints.MapControllers();
});

app.Run();