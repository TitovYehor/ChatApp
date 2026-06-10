namespace ChatApp.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }

    public Guid ChannelId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Channel Channel { get; set; } = null!;

    public User User { get; set; } = null!;
}