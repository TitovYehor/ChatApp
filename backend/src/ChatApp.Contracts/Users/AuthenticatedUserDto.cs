namespace ChatApp.Contracts.Users;

/// <summary>
/// Represents the basic information of an authenticated user
/// </summary>
public class AuthenticatedUserDto
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}