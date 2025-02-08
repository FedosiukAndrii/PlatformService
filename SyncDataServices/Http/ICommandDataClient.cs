using PlatformService.DTOs;

namespace PlatformService.SyncDataServices.Http;

public interface ICommandDataClient
{
    public Task SendPlatformToCommand(PlatformReadDTO platform);
}
