using ChatApp.Contracts.Channels.Requests;
using ChatApp.Contracts.Channels.Responses;
using ChatApp.SignalRTester.Models;

namespace ChatApp.SignalRTester.Clients.Channels;

public interface IChannelApiClient
{
    Task<ApiResult<ChannelResponseDto>> CreateAsync(
        Guid workspaceId,
        CreateChannelRequestDto request);

    Task<ApiResult<IReadOnlyList<ChannelResponseDto>>> GetByWorkspaceIdAsync(
        Guid workspaceId);

    Task<ApiResult<ChannelResponseDto>> GetByIdAsync(
        Guid channelId);
}