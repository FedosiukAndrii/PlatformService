using PlatformService.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient, IAsyncDisposable
{
    private const string TriggerExchange = "trigger";

    private IConnection _connection;
    private IChannel _channel;
    private readonly IConfiguration _configuration;

    private MessageBusClient(IConnection connection, IChannel channel, IConfiguration configuration)
    {
        _connection = connection;
        _channel = channel;
        _configuration = configuration;

        _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
    }

    public static async Task<MessageBusClient> CreateAsync(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQHost"],
            Port = int.Parse(configuration["RabbitMQPort"])
        };

        try
        {
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: TriggerExchange,
                type: ExchangeType.Fanout
            );

            return new MessageBusClient(connection, channel, configuration);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            throw;
        }
    }

    public async Task PublishNewPlatform(PlatformPublishedDTO platformPublishedDTO)
    {
        var message = JsonSerializer.Serialize(platformPublishedDTO);

        if (_channel?.IsOpen == true)
        {
            Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
            await SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ Connection Closed, not sending");
        }
    }

    private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection Shutdown. Attempting to reconnect...");

        var maxRetries = 5;
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQHost"],
                    Port = int.Parse(_configuration["RabbitMQPort"])
                };

                var newConnection = await factory.CreateConnectionAsync();
                var newChannel = await newConnection.CreateChannelAsync();

                await newChannel.ExchangeDeclareAsync(
                    exchange: TriggerExchange,
                    type: ExchangeType.Fanout
                );

                _connection.ConnectionShutdownAsync -= RabbitMQ_ConnectionShutdown;
                await DisposeAsync();

                _connection = newConnection;
                _channel = newChannel;

                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Reconnected to RabbitMQ successfully.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Reconnect attempt {attempt} failed: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        Console.WriteLine("--> Failed to reconnect to RabbitMQ after multiple attempts.");
    }


    private async Task SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await _channel.BasicPublishAsync(
            exchange: TriggerExchange,
            routingKey: "",
            mandatory: false,
            basicProperties: props,
            body: body);

        Console.WriteLine($"--> We have sent {message}");
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("--> MessageBus Disposed");

        if (_channel is { IsOpen: true })
            await _channel.CloseAsync();

        if (_connection is { IsOpen: true })
            await _connection.CloseAsync();
    }
}
