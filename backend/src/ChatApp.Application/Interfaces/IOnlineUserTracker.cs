namespace ChatApp.Application.Interfaces;

public interface IOnlineUserTracker
{
    bool UserConnected(
        Guid userId,
        string connectionId);

    bool UserDisconnected(
        Guid userId,
        string connectionId);

    bool IsOnline(
        Guid userId);

    IReadOnlyCollection<Guid> GetOnlineUserIds();
}