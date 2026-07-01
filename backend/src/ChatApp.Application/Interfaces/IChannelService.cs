using ChatApp.Contracts.Channels.Requests;
using ChatApp.Contracts.Channels.Responses;

namespace ChatApp.Application.Interfaces;

public interface IChannelService
{
    Task<ChannelResponseDto> CreateAsync(
        Guid workspaceId,
        Guid userId,
        CreateChannelRequestDto request);

    Task<ChannelResponseDto> GetByIdAsync(
        Guid channelId,
        Guid userId);

    Task<IReadOnlyCollection<ChannelResponseDto>> GetByWorkspaceIdAsync(
        Guid workspaceId,
        Guid userId);

    Task<ChannelResponseDto> UpdateAsync(
        Guid channelId,
        Guid userId,
        UpdateChannelRequestDto request);

    Task DeleteAsync(
        Guid channelId,
        Guid userId);
}