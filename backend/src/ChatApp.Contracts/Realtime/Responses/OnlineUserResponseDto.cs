namespace ChatApp.Contracts.Realtime.Responses;

/// <summary>
/// Represents a user who is currently online
/// </summary>
public class OnlineUserResponseDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;
}