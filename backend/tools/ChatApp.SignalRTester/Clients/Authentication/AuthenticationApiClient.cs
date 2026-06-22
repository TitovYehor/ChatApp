using System.Net.Http.Json;
using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Authentication.Responses;
using ChatApp.SignalRTester.Configuration;
using Microsoft.Extensions.Options;

namespace ChatApp.SignalRTester.Clients.Authentication;

public class AuthenticationApiClient : IAuthenticationApiClient
{
    private readonly HttpClient _httpClient;

    public AuthenticationApiClient(
        IHttpClientFactory httpClientFactory,
        IOptions<AppSettings> options)
    {
        _httpClient =
            httpClientFactory.CreateClient();

        _httpClient.BaseAddress =
            new Uri(options.Value.ApiBaseUrl);
    }

    public async Task<AuthResponseDto?> LoginAsync(
        LoginRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/login",
            request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var authResponse = await response.Content
            .ReadFromJsonAsync<AuthResponseDto>();

        return authResponse;
    }
}