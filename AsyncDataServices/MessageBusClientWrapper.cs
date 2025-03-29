namespace PlatformService.AsyncDataServices;

public class MessageBusClientWrapper(IConfiguration configuration) : IHostedService, IAsyncDisposable
{
    public IMessageBusClient Client { get; private set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Client = await MessageBusClient.CreateAsync(configuration);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        if (Client != null)
            await Client.DisposeAsync();
    }
}