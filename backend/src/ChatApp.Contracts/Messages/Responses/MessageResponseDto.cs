namespace ChatApp.Contracts.Messages.Responses;

/// <summary>
/// Represents a channel message and its author information
/// </summary>
public class MessageResponseDto
{
    public Guid Id { get; set; }

    public Guid ChannelId { get; set; }

    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update time, or null if the message has not been edited
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}