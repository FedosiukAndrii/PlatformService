using System.Text.Json;

namespace PlatformService.Models;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string Error { get; set; }

    public static OutboxMessage Create<T>(string type, T content)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = type,
            Content = JsonSerializer.Serialize(content),
            CreatedAt = DateTime.UtcNow
        };
    }
}
