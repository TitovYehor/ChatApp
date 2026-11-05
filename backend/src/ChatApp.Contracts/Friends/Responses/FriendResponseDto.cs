namespace ChatApp.Contracts.Friends.Responses;

/// <summary>
/// Represents a friend in the current user's friend list
/// </summary>
public class FriendResponseDto
{
    public Guid UserId { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
}