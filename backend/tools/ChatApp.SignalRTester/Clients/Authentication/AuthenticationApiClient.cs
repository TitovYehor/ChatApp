using ChatApp.Contracts.Authentication.Requests;
using ChatApp.Contracts.Authentication.Responses;
using ChatApp.SignalRTester.Configuration;
using ChatApp.SignalRTester.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

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

    public async Task<ApiResult<AuthResponseDto>> LoginAsync(
        LoginRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/login",
            request);

        if (!response.IsSuccessStatusCode)
        {
            return ApiResult<AuthResponseDto>.Failure(
                $"Request failed ({(int)response.StatusCode})");
        }

        var authResponse = await response.Content
            .ReadFromJsonAsync<AuthResponseDto>();

        return ApiResult<AuthResponseDto>.Success(authResponse!);
    }
}