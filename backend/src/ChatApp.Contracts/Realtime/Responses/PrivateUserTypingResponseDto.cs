namespace ChatApp.Contracts.Realtime.Responses;

/// <summary>
/// Represents a user's typing status in a private conversation
/// </summary>
public class PrivateUserTypingResponseDto
{
    public Guid ConversationId { get; set; }

    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public bool IsTyping { get; set; }
}