using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepository(AppDbContext dbContext) : IPlatformRepository
{
    public bool SaveChanges() => dbContext.SaveChanges() >= 0;

    public IEnumerable<Platform> GetAllPlatforms() => [.. dbContext.Platforms];

    public Platform GetPlatformById(int id) => dbContext.Platforms.FirstOrDefault(p => p.Id == id);

    public void CreatePlatfrom(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);

        dbContext.Platforms.Add(platform);
        dbContext.SaveChanges();
    }
}
