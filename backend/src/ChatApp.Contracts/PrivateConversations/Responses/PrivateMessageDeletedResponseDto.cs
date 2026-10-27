namespace ChatApp.Contracts.PrivateConversations.Responses;

public class PrivateMessageDeletedResponseDto
{
    public Guid MessageId { get; set; }

    public Guid ConversationId { get; set; }
}