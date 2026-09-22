using ChatApp.Application.Authentication;
using ChatApp.Contracts.Authentication.Requests;

namespace ChatApp.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequestDto request);

    Task<AuthResult> LoginAsync(LoginRequestDto request);

    Task<AuthResult> RefreshAsync(string refreshToken);

    Task LogoutAsync(string refreshToken);
}