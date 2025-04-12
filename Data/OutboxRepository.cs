using PlatformService.Models;

namespace PlatformService.Data;

public class OutboxRepository(AppDbContext dbContext) : IOutboxRepository
{
    public async Task AddOutboxMessageAsync(OutboxMessage message)
    {
        await dbContext.OutboxMessages.AddAsync(message);
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}