using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Authentication.Responses;

namespace ChatApp.SignalRTester.Clients.Authentication;

public interface IAuthenticationApiClient
{
    Task<AuthResponseDto?> LoginAsync(
        LoginRequestDto request);
}