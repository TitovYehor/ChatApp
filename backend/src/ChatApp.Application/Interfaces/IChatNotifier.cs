using ChatApp.Contracts.Messages.Responses;
using ChatApp.Contracts.Realtime.Responses;
using ChatApp.Contracts.PrivateConversations.Responses;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines realtime notifications for messages, user presence and typing activity
/// </summary>
public interface IChatNotifier
{
    /// <summary>
    /// Notifies channel members that a message has been created
    /// </summary>
    Task MessageCreatedAsync(
        Guid channelId,
        MessageResponseDto message);

    /// <summary>
    /// Notifies channel members that a message has been updated
    /// </summary>
    Task MessageUpdatedAsync(
        Guid channelId,
        MessageResponseDto message);

    /// <summary>
    /// Notifies channel members that a message has been deleted
    /// </summary>
    Task MessageDeletedAsync(
        Guid channelId,
        MessageDeletedResponseDto response);

    /// <summary>
    /// Notifies conversation participants that a private message has been created
    /// </summary>
    Task PrivateMessageCreatedAsync(
        Guid conversationId,
        PrivateMessageResponseDto message);

    /// <summary>
    /// Notifies conversation participants that a private message has been updated
    /// </summary>
    Task PrivateMessageUpdatedAsync(
        Guid conversationId,
        PrivateMessageResponseDto message);

    /// <summary>
    /// Notifies conversation participants that a private message has been deleted
    /// </summary>
    Task PrivateMessageDeletedAsync(
        Guid conversationId,
        PrivateMessageDeletedResponseDto response);

    /// <summary>
    /// Notifies the specified users about a presence change
    /// </summary>
    Task UserPresenceChangedAsync(
        IEnumerable<Guid> userIds,
        UserPresenceChangedResponseDto response);

    /// <summary>
    /// Sends the current online users snapshot to the specified user
    /// </summary>
    Task OnlineUsersSnapshotAsync(
        Guid userId,
        IReadOnlyCollection<OnlineUserResponseDto> users);

    /// <summary>
    /// Notifies channel members about a user's typing activity
    /// </summary>
    Task UserTypingAsync(
        Guid channelId,
        UserTypingResponseDto response);

    /// <summary>
    /// Notifies conversation participants about a user's typing activity
    /// </summary>
    Task PrivateUserTypingAsync(
        Guid conversationId,
        PrivateUserTypingResponseDto response);
}