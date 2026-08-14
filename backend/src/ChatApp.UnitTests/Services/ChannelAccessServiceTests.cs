using ChatApp.Infrastructure.Services;
using ChatApp.UnitTests.Helpers;

namespace ChatApp.UnitTests.Services;

public class ChannelAccessServiceTests
{
    [Fact]
    public async Task CanAccessChannelAsync_UserIsWorkspaceMember_ReturnsTrue()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = new ChannelAccessService(dbContext);

        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            userId);

        var channel = TestDataFactory.CreateChannel(
            channelId,
            workspaceId);

        workspace.Channels.Add(channel);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var result = await service.CanAccessChannelAsync(
            userId,
            channelId);

        Assert.True(result);
    }

    [Fact]
    public async Task CanAccessChannelAsync_UserIsNotWorkspaceMember_ReturnsFalse()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = new ChannelAccessService(dbContext);

        var memberUserId = Guid.NewGuid();
        var nonMemberUserId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            memberUserId);

        var channel = TestDataFactory.CreateChannel(
            channelId,
            workspaceId);

        workspace.Channels.Add(channel);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var result = await service.CanAccessChannelAsync(
            nonMemberUserId,
            channelId);

        Assert.False(result);
    }
}