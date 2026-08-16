using ChatApp.Application.Interfaces;
using System.Collections.Concurrent;

namespace ChatApp.RealTime.Services;

public class OnlineUserTracker : IOnlineUserTracker
{
    private readonly ConcurrentDictionary<Guid, HashSet<string>> _connections
        = new();

    private readonly object _syncRoot = new();

    public bool UserConnected(
        Guid userId,
        string connectionId)
    {
        lock (_syncRoot)
        {
            if (!_connections.TryGetValue(
                    userId,
                    out var userConnections))
            {
                userConnections = new HashSet<string>();

                _connections[userId] = userConnections;
            }

            var wasOffline = userConnections.Count == 0;

            userConnections.Add(connectionId);

            return wasOffline;
        }
    }

    public bool UserDisconnected(
        Guid userId,
        string connectionId)
    {
        lock (_syncRoot)
        {
            if (!_connections.TryGetValue(
                    userId,
                    out var userConnections))
            {
                return false;
            }

            userConnections.Remove(connectionId);

            if (userConnections.Count > 0)
            {
                return false;
            }

            _connections.TryRemove(
                userId,
                out _);

            return true;
        }
    }

    public bool IsOnline(
        Guid userId)
    {
        return _connections.ContainsKey(userId);
    }

    public IReadOnlyCollection<Guid> GetOnlineUserIds()
    {
        lock (_syncRoot)
        {
            return _connections.Keys.ToList();
        }
    }
}