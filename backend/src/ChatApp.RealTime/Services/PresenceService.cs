using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Realtime.Responses;
using Microsoft.Extensions.Logging;

namespace ChatApp.RealTime.Services;

public class PresenceService : IPresenceService
{
    private readonly IWorkspaceMemberLookupService _lookupService;

    private readonly IOnlineUserTracker _onlineUserTracker;

    private readonly IChatNotifier _chatNotifier;

    private readonly ILogger<PresenceService> _logger;

    public PresenceService(
        IWorkspaceMemberLookupService lookupService,
        IOnlineUserTracker onlineUserTracker,
        IChatNotifier chatNotifier,
        ILogger<PresenceService> logger)
    {
        _lookupService = lookupService;
        _onlineUserTracker = onlineUserTracker;
        _chatNotifier = chatNotifier;
        _logger = logger;
    }

    public async Task UserConnectedAsync(
        Guid userId,
        string connectionId)
    {
        var becameOnline = _onlineUserTracker.UserConnected(
            userId,
            connectionId);

        var onlineUserIds = _onlineUserTracker.GetOnlineUserIds();

        var snapshot = await _lookupService.GetOnlineUsersAsync(
            userId,
            onlineUserIds);
            
        await _chatNotifier.OnlineUsersSnapshotAsync(
            userId,
            snapshot);

        if (!becameOnline)
        {
            return;
        }

        var lookup = await _lookupService
            .GetPresenceLookupAsync(userId);

        var recipients = lookup.RecipientUserIds
            .Where(_onlineUserTracker.IsOnline)
            .ToList();

        if (recipients.Count == 0)
        {
            return;
        }

        await _chatNotifier.UserPresenceChangedAsync(
            recipients,
            new UserPresenceChangedResponseDto
            {
                UserId = lookup.UserId,
                Username = lookup.Username,
                IsOnline = true
            });
    }

    public async Task UserDisconnectedAsync(
        Guid userId,
        string connectionId)
    {
        var becameOffline = _onlineUserTracker.UserDisconnected(
            userId,
            connectionId);

        if (!becameOffline)
        {
            return;
        }

        var lookup = await _lookupService
            .GetPresenceLookupAsync(userId);

        var recipients = lookup.RecipientUserIds
            .Where(_onlineUserTracker.IsOnline)
            .ToList();

        if (recipients.Count == 0)
        {
            return;
        }

        await _chatNotifier.UserPresenceChangedAsync(
            recipients,
            new UserPresenceChangedResponseDto
            {
                UserId = lookup.UserId,
                Username = lookup.Username,
                IsOnline = false
            });
    }
}