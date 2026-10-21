namespace ChatApp.Contracts.PrivateConversations.Responses;

public class PrivateMessageResponseDto
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}