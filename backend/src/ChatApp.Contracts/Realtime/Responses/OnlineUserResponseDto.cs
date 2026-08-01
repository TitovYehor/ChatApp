namespace ChatApp.Contracts.Realtime.Responses;

public class OnlineUserResponseDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;
}