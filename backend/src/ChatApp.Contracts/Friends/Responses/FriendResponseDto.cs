namespace ChatApp.Contracts.Friends.Responses;

public class FriendResponseDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}