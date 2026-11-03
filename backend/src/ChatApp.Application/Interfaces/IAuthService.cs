using ChatApp.Application.Authentication;
using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Users.Requests;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines authentication operations including registration, login,
/// token refresh, logout and password changes
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user and creates an authentication session
    /// </summary>
    Task<AuthResult> RegisterAsync(
        RegisterRequestDto request);

    /// <summary>
    /// Authenticates a user using email and password
    /// </summary>
    Task<AuthResult> LoginAsync(
        LoginRequestDto request);

    /// <summary>
    /// Renews an authentication session using a refresh token
    /// </summary>
    Task<AuthResult> RefreshAsync(
        string refreshToken);

    /// <summary>
    /// Ends an authentication session by revoking its refresh token
    /// </summary>
    Task LogoutAsync(
        string refreshToken);

    /// <summary>
    /// Changes the password of the specified user
    /// </summary>
    Task<AuthResult> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequestDto request);
}