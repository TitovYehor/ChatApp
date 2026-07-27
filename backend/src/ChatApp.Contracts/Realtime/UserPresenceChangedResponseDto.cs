namespace ChatApp.Contracts.Realtime;

public class UserPresenceChangedResponseDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public bool IsOnline { get; set; }
}