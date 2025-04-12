namespace PlatformService.AsyncDataServices;

public interface IMessageBusClient : IAsyncDisposable
{
    Task SendMessage(string message);
}
