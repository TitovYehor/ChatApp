namespace ChatApp.Contracts.Realtime.Responses;

/// <summary>
/// Represents a change in a user's online presence status
/// </summary>
public class UserPresenceChangedResponseDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public bool IsOnline { get; set; }
}