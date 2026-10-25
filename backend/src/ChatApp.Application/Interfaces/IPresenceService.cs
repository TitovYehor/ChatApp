namespace ChatApp.Application.Interfaces;

public interface IPresenceService
{
    Task UserConnectedAsync(
        Guid userId,
        string connectionId);

    Task UserDisconnectedAsync(
        Guid userId,
        string connectionId);

    Task TypingStartedAsync(
        Guid userId,
        Guid channelId);

    Task TypingStoppedAsync(
        Guid userId,
        Guid channelId);

    Task PrivateTypingStartedAsync(
        Guid userId,
        Guid conversationId);

    Task PrivateTypingStoppedAsync(
        Guid userId,
        Guid conversationId);
}