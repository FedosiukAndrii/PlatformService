using PlatformService.Models;

namespace PlatformService.Data;

public interface IOutboxRepository
{
    Task AddOutboxMessageAsync(OutboxMessage message);
    Task SaveChangesAsync();
}