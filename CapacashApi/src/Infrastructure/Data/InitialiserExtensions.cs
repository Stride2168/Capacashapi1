// In a file like `Infrastructure/Data/InitialiserExtensions.cs`

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CapacashApi.Infrastructure.Data;
using Capacash.Infrastructure.Persistence;

namespace CapacashApi.Infrastructure.Data
{
    public static class InitialiserExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var context = serviceProvider.GetRequiredService<AppDbContext>();
            
            // Apply migrations or any custom database initialisation logic here
            await context.Database.MigrateAsync();  // Apply pending migrations
            
            // Optionally, run custom initialization logic like seeding data
        }
    }
}
