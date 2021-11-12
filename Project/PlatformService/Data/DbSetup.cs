using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class DbSetup
    {
        public static void DbPopulation(IApplicationBuilder app, bool isProductionEnvironment)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProductionEnvironment);
            }
        }

        private static void SeedData(AppDbContext appDbContext, bool isProductionEnvironment)
        {
            if (isProductionEnvironment)
            {
                try
                {
                    Console.WriteLine("--> Attempting to apply migrations...");
                    appDbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations {ex.Message}");
                }
            }

            if (!appDbContext.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data in memory...");

                appDbContext.Platforms.AddRange(
                    new Platform() { Name = "Dotnet", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                );

                appDbContext.SaveChanges();
            }
        }
    }
}