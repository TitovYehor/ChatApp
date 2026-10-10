using ChatApp.Application.Authentication;
using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Users.Requests;

namespace ChatApp.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(
        RegisterRequestDto request);

    Task<AuthResult> LoginAsync(
        LoginRequestDto request);

    Task<AuthResult> RefreshAsync(
        string refreshToken);

    Task LogoutAsync(
        string refreshToken);

    Task<AuthResult> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequestDto request);
}