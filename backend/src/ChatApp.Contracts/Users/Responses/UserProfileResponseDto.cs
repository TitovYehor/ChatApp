namespace ChatApp.Contracts.Users.Responses;

/// <summary>
/// Represents the profile information returned to the user themselves
/// </summary>
public class UserProfileResponseDto
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}