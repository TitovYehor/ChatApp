using ChatApp.Application.DTOs.Messages;

namespace ChatApp.Application.Interfaces;

public interface IChatNotifier
{
    Task MessageCreatedAsync(
        Guid channelId,
        MessageResponseDto message);

    Task MessageUpdatedAsync(
        Guid channelId,
        MessageResponseDto message);

    Task MessageDeletedAsync(
        Guid channelId,
        Guid messageId);
}