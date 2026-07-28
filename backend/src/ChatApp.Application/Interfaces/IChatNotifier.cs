using ChatApp.Contracts.Messages.Responses;
using ChatApp.Contracts.Realtime;

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
        MessageDeletedResponseDto response);

    Task UserPresenceChangedAsync(
        IEnumerable<Guid> userIds,
        UserPresenceChangedResponseDto response);
}