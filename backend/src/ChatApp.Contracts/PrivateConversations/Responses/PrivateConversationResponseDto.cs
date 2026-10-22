namespace ChatApp.Contracts.PrivateConversations.Responses;

public class PrivateConversationResponseDto
{
    public Guid Id { get; set; }

    public Guid OtherUserId { get; set; }

    public string OtherUsername { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}