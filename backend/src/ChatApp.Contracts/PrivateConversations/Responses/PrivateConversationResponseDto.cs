namespace ChatApp.Contracts.PrivateConversations.Responses;

/// <summary>
/// Represents a private conversation from the perspective of the current user
/// </summary>
public class PrivateConversationResponseDto
{
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the other participant in the conversation
    /// </summary>
    public Guid OtherUserId { get; set; }

    /// <summary>
    /// Gets or sets the username of the other participant in the conversation
    /// </summary>
    public string OtherUsername { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}