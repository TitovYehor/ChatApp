namespace ChatApp.Domain.Entities;

/// <summary>
/// Represents a message posted by a user in a channel
/// </summary>
public class Message
{
    public Guid Id { get; set; }

    public Guid ChannelId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the time when the message was last updated, or null if it has not been edited
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    public Channel Channel { get; set; } = null!;

    public User User { get; set; } = null!;
}