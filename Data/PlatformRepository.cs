using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepository(AppDbContext dbContext) : IPlatformRepository
{
    public async Task<IDbContextTransaction> BeginTransactionAsync() => await dbContext.Database.BeginTransactionAsync();
    public async Task<bool> SaveChangesAsync() => (await dbContext.SaveChangesAsync()) >= 0;

    public async Task<IEnumerable<Platform>> GetAllPlatformsAsync() => await dbContext.Platforms.ToListAsync();

    public async Task<Platform> GetPlatformByIdAsync(int id) => await dbContext.Platforms.FirstOrDefaultAsync(p => p.Id == id);

    public async Task CreatePlatfromAsync(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);

        await dbContext.Platforms.AddAsync(platform);
    }
}
