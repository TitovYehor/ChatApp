using ChatApp.Contracts.Channels.Requests;
using ChatApp.Contracts.Channels.Responses;
using ChatApp.SignalRTester.Configuration;
using ChatApp.SignalRTester.Models;
using ChatApp.SignalRTester.Session;
using Microsoft.Extensions.Options;

namespace ChatApp.SignalRTester.Clients.Channels;

public class ChannelApiClient: ApiClientBase, IChannelApiClient
{
    public ChannelApiClient(
        IHttpClientFactory factory,
        IOptions<AppSettings> options,
        UserSession session)
        : base(factory.CreateClient(), session)
    {
        HttpClient.BaseAddress = new Uri(options.Value.ApiBaseUrl);
    }

    public Task<ApiResult<ChannelResponseDto>> CreateAsync(
        Guid workspaceId,
        CreateChannelRequestDto request)
    {
        return PostAsync<CreateChannelRequestDto, ChannelResponseDto>(
            $"api/workspaces/{workspaceId}/channels",
            request);
    }

    public Task<ApiResult<IReadOnlyList<ChannelResponseDto>>> GetByWorkspaceIdAsync(
        Guid workspaceId)
    {
        return GetAsync<IReadOnlyList<ChannelResponseDto>>(
            $"api/workspaces/{workspaceId}/channels");
    }

    public Task<ApiResult<ChannelResponseDto>> GetByIdAsync(
        Guid channelId)
    {
        return GetAsync<ChannelResponseDto>(
            $"api/channels/{channelId}");
    }
}