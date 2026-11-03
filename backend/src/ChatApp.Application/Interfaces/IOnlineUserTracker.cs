namespace ChatApp.Application.Interfaces;

/// <summary>
/// Tracks active SignalR connections and online status for users
/// </summary>
public interface IOnlineUserTracker
{
    /// <summary>
    /// Registers a user's connection and indicates whether this is their first active connection
    /// </summary>
    bool UserConnected(
        Guid userId,
        string connectionId);

    /// <summary>
    /// Removes a user's connection and indicates whether they no longer have active connections
    /// </summary>
    bool UserDisconnected(
        Guid userId,
        string connectionId);

    /// <summary>
    /// Determines whether a user currently has an active connection
    /// </summary>
    bool IsOnline(
        Guid userId);

    /// <summary>
    /// Gets the IDs of all users with active connections
    /// </summary>
    IReadOnlyCollection<Guid> GetOnlineUserIds();
}