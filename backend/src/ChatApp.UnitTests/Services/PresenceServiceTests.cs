using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Realtime.Responses;
using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.RealTime.Services;
using Moq;

namespace ChatApp.UnitTests.Services;

public class PresenceServiceTests
{
    private readonly Mock<IWorkspaceMemberLookupService>
        _lookupServiceMock = new();

    private readonly Mock<IOnlineUserTracker>
        _onlineUserTrackerMock = new();

    private readonly Mock<IChatNotifier>
        _chatNotifierMock = new();

    [Fact]
    public async Task UserConnectedAsync_FirstConnection_NotifiesSnapshotAndPresenceChange()
    {
        var userId = Guid.NewGuid();
        var connectionId = "connection-1";
        var recipientId = Guid.NewGuid();

        var onlineUserIds = new List<Guid>
        {
            userId,
            recipientId
        };

        var snapshot = new List<OnlineUserResponseDto>
        {
            new()
            {
                UserId = recipientId,
                Username = "recipient"
            }
        };

        _onlineUserTrackerMock
            .Setup(x => x.UserConnected(
                userId,
                connectionId))
            .Returns(true);

        _onlineUserTrackerMock
            .Setup(x => x.GetOnlineUserIds())
            .Returns(onlineUserIds);

        _lookupServiceMock
            .Setup(x => x.GetOnlineUsersAsync(
                userId,
                onlineUserIds))
            .ReturnsAsync(snapshot);

        _lookupServiceMock
            .Setup(x => x.GetPresenceLookupAsync(userId))
            .ReturnsAsync(new PresenceLookupResponseDto
            {
                UserId = userId,
                Username = "testuser",
                RecipientUserIds = new List<Guid>
                {
                    recipientId
                }
            });

        _onlineUserTrackerMock
            .Setup(x => x.IsOnline(recipientId))
            .Returns(true);

        var service = CreatePresenceService();

        await service.UserConnectedAsync(
            userId,
            connectionId);

        _chatNotifierMock.Verify(
            x => x.OnlineUsersSnapshotAsync(
                userId,
                snapshot),
            Times.Once);

        _chatNotifierMock.Verify(
            x => x.UserPresenceChangedAsync(
                It.Is<IEnumerable<Guid>>(ids =>
                    ids.Count() == 1 &&
                    ids.Contains(recipientId)),
                It.Is<UserPresenceChangedResponseDto>(response =>
                    response.UserId == userId &&
                    response.Username == "testuser" &&
                    response.IsOnline)),
            Times.Once);
    }

    [Fact]
    public async Task UserConnectedAsync_AdditionalConnection_DoesNotNotifyPresenceChange()
    {
        var userId = Guid.NewGuid();
        var connectionId = "connection-2";

        var onlineUserIds = new List<Guid>
        {
            userId
        };

        var snapshot = new List<OnlineUserResponseDto>();

        _onlineUserTrackerMock
            .Setup(x => x.UserConnected(
                userId,
                connectionId))
            .Returns(false);

        _onlineUserTrackerMock
            .Setup(x => x.GetOnlineUserIds())
            .Returns(onlineUserIds);

        _lookupServiceMock
            .Setup(x => x.GetOnlineUsersAsync(
                userId,
                onlineUserIds))
            .ReturnsAsync(snapshot);

        var service = CreatePresenceService();

        await service.UserConnectedAsync(
            userId,
            connectionId);

        _chatNotifierMock.Verify(
            x => x.OnlineUsersSnapshotAsync(
                userId,
                snapshot),
            Times.Once);

        _lookupServiceMock.Verify(
            x => x.GetPresenceLookupAsync(userId),
            Times.Never);

        _chatNotifierMock.Verify(
            x => x.UserPresenceChangedAsync(
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<UserPresenceChangedResponseDto>()),
            Times.Never);
    }

    private PresenceService CreatePresenceService()
    {
        return new PresenceService(
            _lookupServiceMock.Object,
            _onlineUserTrackerMock.Object,
            _chatNotifierMock.Object);
    }
}