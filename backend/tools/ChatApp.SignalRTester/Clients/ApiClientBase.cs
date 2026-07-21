using ChatApp.Contracts.Common;
using ChatApp.SignalRTester.Models;
using ChatApp.SignalRTester.Session;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ChatApp.SignalRTester.Clients;

public abstract class ApiClientBase
{
    protected readonly HttpClient HttpClient;

    protected readonly UserSession UserSession;

    protected ApiClientBase(
        HttpClient httpClient,
        UserSession userSession)
    {
        HttpClient = httpClient;
        UserSession = userSession;
    }

    protected Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string url,
        TRequest request)
    {
        var requestMessage = new HttpRequestMessage(
                HttpMethod.Post,
                url)
            {
                Content = JsonContent.Create(request)
            };

        return SendAsync<TResponse>(requestMessage);
    }

    protected Task<ApiResult<TResponse>> GetAsync<TResponse>(
        string url)
    {
        return SendAsync<TResponse>(
            new HttpRequestMessage(
                HttpMethod.Get,
                url));
    }

    protected Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(
        string url,
        TRequest request)
    {
        return SendAsync<TResponse>(
            new HttpRequestMessage(
                HttpMethod.Put,
                url)
            {
                Content =
                    JsonContent.Create(request)
            });
    }

    protected async Task<ApiResult<bool>> DeleteRequestAsync(string url)
    {
        try
        {
            var response = await HttpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content
                    .ReadFromJsonAsync<ErrorResponse>();

                if (error != null)
                {
                    return ApiResult<bool>.Failure(
                        error.Message);
                }

                return ApiResult<bool>.Failure(
                    $"HTTP {(int)response.StatusCode}");
            }

            return ApiResult<bool>.Success(true);
        }
        catch (HttpRequestException)
        {
            return ApiResult<bool>.Failure(
                "Unable to connect to the server");
        }
        catch (TaskCanceledException)
        {
            return ApiResult<bool>.Failure(
                "The request timed out");
        }
        catch (Exception ex)
        {
            return ApiResult<bool>.Failure(
                $"Unexpected error: {ex.Message}");
        }
    }

    protected async Task<ApiResult<bool>> DeleteAsync<TRequest>(
        string url,
        TRequest request)
    {
        try
        {
            var message = new HttpRequestMessage(
                HttpMethod.Delete,
                url)
            {
                Content = JsonContent.Create(request)
            };

            if (UserSession.IsAuthenticated)
            {
                message.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        UserSession.AccessToken);
            }

            var response = await HttpClient.SendAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content
                    .ReadFromJsonAsync<ErrorResponse>();

                return ApiResult<bool>.Failure(
                    error?.Message ??
                    $"HTTP {(int)response.StatusCode}");
            }

            return ApiResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return ApiResult<bool>.Failure(ex.Message);
        }
    }

    protected async Task<ApiResult<TResponse>> SendAsync<TResponse>(
        HttpRequestMessage request)
    {
        try
        {
            if (UserSession.IsAuthenticated)
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    UserSession.AccessToken);
            }

            var response = await HttpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content
                    .ReadFromJsonAsync<ErrorResponse>();

                if (error != null)
                {
                    return ApiResult<TResponse>.Failure(
                        error.Message);
                }

                return ApiResult<TResponse>.Failure(
                    $"HTTP {(int)response.StatusCode}");
            }

            var dto = await response.Content
                .ReadFromJsonAsync<TResponse>();

            if (dto == null)
            {
                return ApiResult<TResponse>.Failure(
                    "Server returned an empty response");
            }

            return ApiResult<TResponse>.Success(dto);
        }
        catch (HttpRequestException)
        {
            return ApiResult<TResponse>.Failure(
                "Unable to connect to the server");
        }
        catch (TaskCanceledException)
        {
            return ApiResult<TResponse>.Failure(
                "The request timed out");
        }
        catch (Exception ex)
        {
            return ApiResult<TResponse>.Failure(
                $"Unexpected error: {ex.Message}");
        }
    }

    protected async Task<ApiResult<bool>> PostEmptyAsync(
        string url)
    {
        try
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                url);

            if (UserSession.IsAuthenticated)
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer",
                        UserSession.AccessToken);
            }

            var response = await HttpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content
                    .ReadFromJsonAsync<ErrorResponse>();

                if (error != null)
                {
                    return ApiResult<bool>.Failure(
                        error.Message);
                }

                return ApiResult<bool>.Failure(
                    $"HTTP {(int)response.StatusCode}");
            }

            return ApiResult<bool>.Success(true);
        }
        catch (HttpRequestException)
        {
            return ApiResult<bool>.Failure(
                "Unable to connect to the server");
        }
        catch (TaskCanceledException)
        {
            return ApiResult<bool>.Failure(
                "The request timed out");
        }
        catch (Exception ex)
        {
            return ApiResult<bool>.Failure(
                $"Unexpected error: {ex.Message}");
        }
    }

    protected async Task<ApiResult<bool>> PostEmptyAsync<TRequest>(
        string url,
        TRequest request)
    {
        try
        {
            var requestMessage = new HttpRequestMessage(
                HttpMethod.Post,
                url)
            {
                Content = JsonContent.Create(request)
            };

            if (UserSession.IsAuthenticated)
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        UserSession.AccessToken);
            }

            var response = await HttpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content
                    .ReadFromJsonAsync<ErrorResponse>();

                if (error != null)
                {
                    return ApiResult<bool>.Failure(error.Message);
                }

                return ApiResult<bool>.Failure(
                    $"HTTP {(int)response.StatusCode}");
            }

            return ApiResult<bool>.Success(true);
        }
        catch (HttpRequestException)
        {
            return ApiResult<bool>.Failure("Unable to connect to the server");
        }
        catch (TaskCanceledException)
        {
            return ApiResult<bool>.Failure("The request timed out");
        }
        catch (Exception ex)
        {
            return ApiResult<bool>.Failure($"Unexpected error: {ex.Message}");
        }
    }

    protected static string AppendQueryString(
        string url,
        IDictionary<string, string?> parameters)
    {
        var query = string.Join(
            "&",
            parameters
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(x =>
                    $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value!)}"));

        return string.IsNullOrEmpty(query)
            ? url
            : $"{url}?{query}";
    }
}