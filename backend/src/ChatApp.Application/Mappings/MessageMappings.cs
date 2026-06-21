using ChatApp.Contracts.Messages.Responses;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mappings;

public static class MessageMappings
{
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