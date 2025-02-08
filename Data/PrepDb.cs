using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(this IApplicationBuilder app, bool isProduction)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();

        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
    }

    private static void SeedData(AppDbContext db, bool isProduction)
    {
        Console.WriteLine(isProduction);
        if (isProduction)
        {
            Console.WriteLine("--> Attempting to apply migrations...");
            try
            {
                db.Database.Migrate();  
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Couldn't run migration: {ex.Message}");
            }
        }


        if (!db.Platforms.Any())
        {
            Console.WriteLine("--> Seeding data...");

            db.Platforms.AddRange(
                new Platform() { Name = ".NET", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "SQL Server", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "Kubernrtes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
            );

            db.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> We already have data");
        }
    }
}
