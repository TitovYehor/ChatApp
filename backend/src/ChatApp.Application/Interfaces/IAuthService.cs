using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Authentication.Responses;

namespace ChatApp.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
}