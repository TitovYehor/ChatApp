namespace ChatApp.Contracts.Realtime.Responses;

public class UserTypingResponseDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public Guid ChannelId { get; set; }

    public bool IsTyping { get; set; }
}