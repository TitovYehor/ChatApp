namespace ChatApp.Contracts.PrivateConversations.Responses;

/// <summary>
/// Identifies a deleted private message and the conversation it belonged to
/// </summary>
public class PrivateMessageDeletedResponseDto
{
    public Guid MessageId { get; set; }

    public Guid ConversationId { get; set; }
}