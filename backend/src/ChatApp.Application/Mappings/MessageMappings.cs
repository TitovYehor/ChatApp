using ChatApp.Contracts.Messages.Responses;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mappings;

/// <summary>
/// Defines mappings between message domain entities and response DTOs
/// </summary>
public static class MessageMappings
{
    /// <summary>
    /// Maps a message entity to its response DTO, including the author's username
    /// </summary>
    public static MessageResponseDto ToDto(
        this Message message)
    {
        return new MessageResponseDto
        {
            Id = message.Id,
            ChannelId = message.ChannelId,
            UserId = message.UserId,
            Username = message.User.Username,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt
        };
    }
}