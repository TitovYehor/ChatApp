using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Authentication.Responses;
using ChatApp.SignalRTester.Configuration;
using ChatApp.SignalRTester.Models;
using Microsoft.Extensions.Options;

namespace ChatApp.SignalRTester.Clients.Authentication;

public class AuthenticationApiClient : ApiClientBase, IAuthenticationApiClient
{
    public AuthenticationApiClient(
        IHttpClientFactory httpClientFactory,
        IOptions<AppSettings> options)
        : base(httpClientFactory.CreateClient())
    {
        HttpClient.BaseAddress = new Uri(options.Value.ApiBaseUrl);
    }

    public Task<ApiResult<AuthResponseDto>> LoginAsync(
        LoginRequestDto request)
    {
        return PostAsync<LoginRequestDto, AuthResponseDto>(
            "api/auth/login",
            request);
    }
}