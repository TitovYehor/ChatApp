using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Authentication.Responses;
using ChatApp.SignalRTester.Models;

namespace ChatApp.SignalRTester.Clients.Authentication;

public interface IAuthenticationApiClient
{
    Task<ApiResult<AuthResponseDto>> LoginAsync(
        LoginRequestDto request);
}