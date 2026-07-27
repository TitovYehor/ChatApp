using ChatApp.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ChatApp.RealTime.Services;

public class PresenceService : IPresenceService
{
    private readonly IOnlineUserTracker _onlineUserTracker;

    private readonly ILogger<PresenceService> _logger;

    public PresenceService(
        IOnlineUserTracker onlineUserTracker,
        ILogger<PresenceService> logger)
    {
        _onlineUserTracker = onlineUserTracker;
        _logger = logger;
    }

    public Task UserConnectedAsync(
        Guid userId,
        string connectionId)
    {
        var becameOnline = _onlineUserTracker.UserConnected(
            userId,
            connectionId);

        if (becameOnline)
        {
            _logger.LogInformation("User {UserId} is now online",
                userId);
        }

        return Task.CompletedTask;
    }

    public Task UserDisconnectedAsync(
        Guid userId,
        string connectionId)
    {
        var becameOffline = _onlineUserTracker.UserDisconnected(
            userId,
            connectionId);

        if (becameOffline)
        {
            _logger.LogInformation("User {UserId} went offline",
                userId);
        }

        return Task.CompletedTask;
    }
}