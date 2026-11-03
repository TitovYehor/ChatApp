namespace ChatApp.Application.Interfaces;

/// <summary>
/// Handles user presence and typing activity for channels and private conversations
/// </summary>
public interface IPresenceService
{
    /// <summary>
    /// Processes a user's new connection and updates their presence
    /// </summary>
    Task UserConnectedAsync(
        Guid userId,
        string connectionId);

    /// <summary>
    /// Processes a user's disconnected connection and updates their presence
    /// </summary>
    Task UserDisconnectedAsync(
        Guid userId,
        string connectionId);

    /// <summary>
    /// Notifies channel participants that a user has started typing
    /// </summary>
    Task TypingStartedAsync(
        Guid userId,
        Guid channelId);

    /// <summary>
    /// Notifies channel participants that a user has stopped typing
    /// </summary>
    Task TypingStoppedAsync(
        Guid userId,
        Guid channelId);

    /// <summary>
    /// Notifies conversation participants that a user has started typing
    /// </summary>
    Task PrivateTypingStartedAsync(
        Guid userId,
        Guid conversationId);

    /// <summary>
    /// Notifies conversation participants that a user has stopped typing
    /// </summary>
    Task PrivateTypingStoppedAsync(
        Guid userId,
        Guid conversationId);
}