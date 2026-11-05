using ChatApp.Contracts.Users;

namespace ChatApp.Contracts.Authentication.Responses;

/// <summary>
/// Contains the authentication token and information about the authenticated user
/// </summary>
public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public AuthenticatedUserDto User { get; set; } = new();
}