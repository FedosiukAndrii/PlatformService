using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepository(AppDbContext dbContext) : IPlatformRepository
{
    public async Task<bool> SaveChangesAsync() => (await dbContext.SaveChangesAsync()) >= 0;

    public async Task<IEnumerable<Platform>> GetAllPlatformsAsync() => await dbContext.Platforms.ToListAsync();

    public async Task<Platform> GetPlatformByIdAsync(int id) => await dbContext.Platforms.FirstOrDefaultAsync(p => p.Id == id);

    public async Task CreatePlatfromAsync(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);

        dbContext.Platforms.Add(platform);
        await dbContext.SaveChangesAsync();
    }
}
