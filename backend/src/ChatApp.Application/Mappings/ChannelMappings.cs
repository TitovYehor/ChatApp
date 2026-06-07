using ChatApp.Application.DTOs.Channels;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mappings;

public static class ChannelMappings
{
    public static ChannelResponseDto ToDto(
        this Channel channel)
    {
        return new ChannelResponseDto
        {
            Id = channel.Id,
            WorkspaceId = channel.WorkspaceId,
            Name = channel.Name,
            Type = (int)channel.Type,
            CreatedAt = channel.CreatedAt
        };
    }
}