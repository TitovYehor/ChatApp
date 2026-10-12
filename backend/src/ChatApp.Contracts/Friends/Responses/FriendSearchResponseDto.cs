namespace ChatApp.Contracts.Friends.Responses;

public class FriendSearchResponseDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsFriend { get; set; }
}