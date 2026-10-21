namespace ChatApp.Domain.Entities;

public class PrivateMessage
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public PrivateConversation Conversation { get; set; } = null!;
    public User User { get; set; } = null!;
}