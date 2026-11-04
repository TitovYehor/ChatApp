using ChatApp.Contracts.Authentication.Responses;

namespace ChatApp.Application.Authentication;

/// <summary>
/// Contains the authentication response and refresh token returned by the authentication service
/// </summary>
public class AuthResult
{
    /// <summary>
    /// Gets or sets the authentication response containing user and token information
    /// </summary>
    public AuthResponseDto Response { get; set; } = new();

    /// <summary>
    /// Gets or sets the refresh token issued during authentication
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}