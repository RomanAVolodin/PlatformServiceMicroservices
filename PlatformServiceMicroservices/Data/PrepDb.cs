using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;
using System;
using System.Linq;

namespace PlatformService.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }

        }

        private static void SeedData(AppDbContext context)
        {
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