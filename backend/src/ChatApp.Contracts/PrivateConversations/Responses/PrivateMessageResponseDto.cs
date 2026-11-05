namespace ChatApp.Contracts.PrivateConversations.Responses;

/// <summary>
/// Represents a message exchanged in a private conversation
/// </summary>
public class PrivateMessageResponseDto
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update time, or null if the message has not been edited
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}