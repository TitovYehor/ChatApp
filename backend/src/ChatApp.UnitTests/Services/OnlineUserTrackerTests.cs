using ChatApp.RealTime.Services;

namespace ChatApp.UnitTests.Services;

public class OnlineUserTrackerTests
{
    private readonly OnlineUserTracker _sut = new();

    [Fact]
    public void UserConnected_FirstConnection_ReturnsTrueAndUserIsOnline()
    {
        var userId = Guid.NewGuid();

        var becameOnline = _sut.UserConnected(
            userId,
            "connection-1");

        Assert.True(becameOnline);
        Assert.True(_sut.IsOnline(userId));
        Assert.Contains(
            userId,
            _sut.GetOnlineUserIds());
    }

    [Fact]
    public void UserConnected_SecondConnection_ReturnsFalseAndUserRemainsOnline()
    {
        var userId = Guid.NewGuid();

        _sut.UserConnected(userId, "connection-1");

        var becameOnline = _sut.UserConnected(
            userId,
            "connection-2");

        Assert.False(becameOnline);
        Assert.True(_sut.IsOnline(userId));

        Assert.Contains(
            userId,
            _sut.GetOnlineUserIds());
    }

    [Fact]
    public void UserDisconnected_WhenOtherConnectionsRemain_ReturnsFalseAndUserStaysOnline()
    {
        var userId = Guid.NewGuid();

        _sut.UserConnected(userId, "connection-1");
        _sut.UserConnected(userId, "connection-2");

        var becameOffline = _sut.UserDisconnected(
            userId,
            "connection-1");

        Assert.False(becameOffline);
        Assert.True(_sut.IsOnline(userId));

        Assert.Contains(
            userId,
            _sut.GetOnlineUserIds());
    }

    [Fact]
    public void UserDisconnected_LastConnection_ReturnsTrueAndUserBecomesOffline()
    {
        var userId = Guid.NewGuid();

        _sut.UserConnected(userId, "connection-1");

        var becameOffline = _sut.UserDisconnected(
            userId,
            "connection-1");

        Assert.True(becameOffline);
        Assert.False(_sut.IsOnline(userId));

        Assert.DoesNotContain(
            userId,
            _sut.GetOnlineUserIds());
    }

    [Fact]
    public void UserDisconnected_UnknownUser_ReturnsFalse()
    {
        var userId = Guid.NewGuid();

        var becameOffline = _sut.UserDisconnected(
            userId,
            "connection-1");

        Assert.False(becameOffline);
        Assert.False(_sut.IsOnline(userId));

        Assert.Empty(
            _sut.GetOnlineUserIds());
    }

    [Fact]
    public void GetOnlineUserIds_ReturnsAllOnlineUsers()
    {
        var firstUserId = Guid.NewGuid();
        var secondUserId = Guid.NewGuid();

        _sut.UserConnected(firstUserId, "connection-1");
        _sut.UserConnected(secondUserId, "connection-2");

        var onlineUserIds = _sut.GetOnlineUserIds();

        Assert.Equal(2, onlineUserIds.Count);
        Assert.Contains(firstUserId, onlineUserIds);
        Assert.Contains(secondUserId, onlineUserIds);
    }
}