namespace ChatApp.Domain.Entities;

/// <summary>
/// Represents a message sent by a user in a private conversation
/// </summary>
public class PrivateMessage
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the time when the message was last updated, or null if it has not been edited
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    public PrivateConversation Conversation { get; set; } = null!;
    public User User { get; set; } = null!;
}