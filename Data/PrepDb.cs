using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();

        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
    }

    private static void SeedData(AppDbContext db)
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


        if (!db.Platforms.Any())
        {
            Console.WriteLine("--> Seeding data...");

            db.Platforms.AddRange(
                new Platform() { Name = ".NET", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "SQL Server", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
            );

            db.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> We already have data");
        }
    }
}
