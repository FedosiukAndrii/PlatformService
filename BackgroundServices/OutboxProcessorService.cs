using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;

namespace PlatformService.BackgroundServices;

public class OutboxProcessorService(IServiceProvider serviceProvider, ILogger<OutboxProcessorService> logger, IMessageBusClient messageBus) : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(10));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessages(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing outbox messages");
            }
        }
    }

    private async Task ProcessOutboxMessages(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var messages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(50)
            .ToListAsync(stoppingToken);

        foreach (var message in messages)
        {
            const int maxRetry = 3;
            int attempt = 0;
            bool sent = false;
            TimeSpan delay = TimeSpan.FromSeconds(1);

            while (!sent && attempt < maxRetry && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await messageBus.SendMessage(message.Content);
                    sent = true;
                }
                catch (Exception ex)
                {
                    attempt++;
                    logger.LogWarning(ex, "Transient error sending message {MessageId}, attempt {Attempt}", message.Id, attempt);
                    if (attempt < maxRetry)
                    {
                        await Task.Delay(delay, stoppingToken);
                        delay *= 2;
                    }
                    else
                    {
                        logger.LogError("Failed to send message {MessageId} after {MaxRetry} attempts", message.Id, maxRetry);
                        message.Error = ex.Message;
                    }
                }
            }

            message.ProcessedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}