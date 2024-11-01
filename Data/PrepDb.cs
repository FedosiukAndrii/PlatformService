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
        if (!db.Platforms.Any())
        {
            Console.WriteLine("--> Seeding data...");

            db.Platforms.AddRange(
                new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "Kubernrtes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
            );
        }
        else
        {
            Console.WriteLine("--> We already have data");
        }
    }
}
