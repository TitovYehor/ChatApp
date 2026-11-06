namespace ChatApp.Contracts.Users.Responses;

/// <summary>
/// Represents the user profile information available to other users
/// </summary>
public class PublicUserProfileResponseDto
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}