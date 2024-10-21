using Avans.FoodWaste.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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


            //New code
            await context.Database.EnsureDeletedAsync(cancellationToken); //Delete database

            await context.Database.MigrateAsync(cancellationToken); //Create and migrate

            FoodWasteDataSeeder.SeedData(context); //Seed the database
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}