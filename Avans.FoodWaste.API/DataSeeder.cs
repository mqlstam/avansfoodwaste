using Avans.FoodWaste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Avans.FoodWaste.API
{
    public class DataSeeder : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DataSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FoodWasteDbContext>();
            var serviceProvider2 = scope.ServiceProvider;

            await context.Database.EnsureDeletedAsync(cancellationToken); 
            await context.Database.MigrateAsync(cancellationToken); 

            await FoodWasteDataSeeder.SeedDataAsync(context, serviceProvider2); // Call SeedDataAsync
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}   