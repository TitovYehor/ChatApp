using ChatApp.Contracts.Messages.Responses;
using ChatApp.Contracts.Realtime.Responses;
using ChatApp.Contracts.PrivateConversations.Responses;

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

    Task PrivateMessageCreatedAsync(
        Guid conversationId,
        PrivateMessageResponseDto message);

    Task PrivateMessageUpdatedAsync(
        Guid conversationId,
        PrivateMessageResponseDto message);

    Task PrivateMessageDeletedAsync(
        Guid conversationId,
        PrivateMessageDeletedResponseDto response);

    Task UserPresenceChangedAsync(
        IEnumerable<Guid> userIds,
        UserPresenceChangedResponseDto response);

    Task OnlineUsersSnapshotAsync(
        Guid userId,
        IReadOnlyCollection<OnlineUserResponseDto> users);

    Task UserTypingAsync(
        Guid channelId,
        UserTypingResponseDto response);

    Task PrivateUserTypingAsync(
        Guid conversationId,
        PrivateUserTypingResponseDto response);
}