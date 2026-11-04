namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines realtime notifications for friendship changes
/// </summary>
public interface IFriendshipNotifier
{
    /// <summary>
    /// Notifies the specified users that a friendship has changed
    /// </summary>
    Task FriendshipChangedAsync(
        IReadOnlyCollection<Guid> userIds);
}