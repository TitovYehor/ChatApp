using ChatApp.Application.DTOs.Channels;

namespace ChatApp.Application.Interfaces;

public interface IChannelService
{
    Task<ChannelResponseDto> CreateAsync(
        Guid workspaceId,
        Guid userId,
        CreateChannelRequestDto request);
}