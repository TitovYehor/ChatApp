namespace ChatApp.Contracts.Friends.Responses;

/// <summary>
/// Represents a user found during friend search and indicates whether they are already a friend
/// </summary>
public class FriendSearchResponseDto
{
    public Guid UserId { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public bool IsFriend { get; set; }
}