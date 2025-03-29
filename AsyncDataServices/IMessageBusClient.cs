using PlatformService.DTOs;

namespace PlatformService.AsyncDataServices;

public interface IMessageBusClient : IAsyncDisposable
{
    Task PublishNewPlatform(PlatformPublishedDTO platformPublishedDTO);
}
