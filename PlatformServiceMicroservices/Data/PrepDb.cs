using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;
using System;
using System.Linq;

namespace PlatformService.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }

        }

        private static void SeedData(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("--> Applying migrations");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
                
            }

            if (!context.Platforms.Any())
            {
                Console.WriteLine("---> Seeding data ...");

                context.Platforms.AddRange(
                    new Platform() { Name="Dotnet", Publisher="Microsoft", Cost="Free"},
                    new Platform() { Name="SQL Server Express", Publisher="Microsoft", Cost="Free"},
                    new Platform() { Name="Kubernetes", Publisher="Cloud Native", Cost="Free"}
                );

                context.SaveChanges();
            } else
            {
                Console.WriteLine("---> We already hav data");
            }
        }
    }
}