namespace ChatApp.Contracts.Realtime;

public class OnlineUserResponseDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;
}