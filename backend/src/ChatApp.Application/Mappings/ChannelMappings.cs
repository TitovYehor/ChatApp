using ChatApp.Contracts.Channels.Responses;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mappings;

/// <summary>
/// Defines mappings between channel domain entities and response DTOs
/// </summary>
public static class ChannelMappings
{
    /// <summary>
    /// Maps a channel entity to its response DTO
    /// </summary>
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