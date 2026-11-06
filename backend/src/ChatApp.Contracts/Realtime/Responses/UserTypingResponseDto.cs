namespace ChatApp.Contracts.Realtime.Responses;

/// <summary>
/// Represents a user's typing status in a channel
/// </summary>
public class UserTypingResponseDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public Guid ChannelId { get; set; }

    public bool IsTyping { get; set; }
}