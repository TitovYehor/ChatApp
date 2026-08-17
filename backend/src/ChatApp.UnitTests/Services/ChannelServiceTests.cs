using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Channels.Requests;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Services;
using ChatApp.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ChatApp.UnitTests.Services;

public class ChannelServiceTests
{
    private readonly Mock<IWorkspaceAuthorizationService> _workspaceAuthorizationMock;

    public ChannelServiceTests()
    {
        _workspaceAuthorizationMock = new Mock<IWorkspaceAuthorizationService>();
    }

    [Fact]
    public async Task CreateAsync_AuthorizedUser_CreatesChannel()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateChannelService(dbContext);

        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            userId,
            WorkspaceRole.Admin);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        _workspaceAuthorizationMock
            .Setup(x => x.EnsureCanCreateChannelAsync(
                workspaceId,
                userId))
            .Returns(Task.CompletedTask);

        var request = new CreateChannelRequestDto
        {
            Name = "   general   "
        };

        var result = await service.CreateAsync(
            workspaceId,
            userId,
            request);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(workspaceId, result.WorkspaceId);
        Assert.Equal("general", result.Name);
        Assert.Equal((int)ChannelType.Text, result.Type);
        Assert.NotEqual(default, result.CreatedAt);

        var savedChannel = await dbContext.Channels
            .FirstOrDefaultAsync(x => x.Id == result.Id);

        Assert.NotNull(savedChannel);
        Assert.Equal(workspaceId, savedChannel.WorkspaceId);
        Assert.Equal("general", savedChannel.Name);
        Assert.Equal(ChannelType.Text, savedChannel.Type);

        _workspaceAuthorizationMock.Verify(
            x => x.EnsureCanCreateChannelAsync(
                workspaceId,
                userId),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_UnauthorizedUser_ThrowsForbiddenException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateChannelService(dbContext);

        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _workspaceAuthorizationMock
            .Setup(x => x.EnsureCanCreateChannelAsync(
                workspaceId,
                userId))
            .ThrowsAsync(
                new ForbiddenException(
                    "Only workspace administrators can create channels"));

        var request = new CreateChannelRequestDto
        {
            Name = "general"
        };

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.CreateAsync(
                workspaceId,
                userId,
                request));

        Assert.Equal(
            "Only workspace administrators can create channels",
            exception.Message);

        Assert.Empty(
            await dbContext.Channels.ToListAsync());

        _workspaceAuthorizationMock.Verify(
            x => x.EnsureCanCreateChannelAsync(
                workspaceId,
                userId),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_MemberGetsChannel()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateChannelService(dbContext);

        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            userId,
            WorkspaceRole.Member);

        var channel = TestDataFactory.CreateChannel(
            channelId,
            workspaceId,
            "general",
            ChannelType.Text);

        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);

        await dbContext.SaveChangesAsync();

        var result = await service.GetByIdAsync(
            channelId,
            userId);

        Assert.Equal(channelId, result.Id);
        Assert.Equal(workspaceId, result.WorkspaceId);
        Assert.Equal("general", result.Name);
        Assert.Equal((int)ChannelType.Text, result.Type);
    }

    [Fact]
    public async Task GetByIdAsync_NonMemberThrowsNotFoundException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateChannelService(dbContext);

        var memberId = Guid.NewGuid();
        var nonMemberId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            memberId,
            WorkspaceRole.Member);

        var channel = TestDataFactory.CreateChannel(
            channelId,
            workspaceId,
            "general",
            ChannelType.Text);

        dbContext.Workspaces.Add(workspace);
        dbContext.Channels.Add(channel);

        await dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetByIdAsync(
                channelId,
                nonMemberId));
    }

    [Fact]
    public async Task GetByWorkspaceIdAsync_MemberGetsChannelsOrderedByName()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateChannelService(dbContext);

        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            userId,
            WorkspaceRole.Member);

        dbContext.Workspaces.Add(workspace);

        dbContext.Channels.AddRange(
            TestDataFactory.CreateChannel(
                Guid.NewGuid(),
                workspaceId,
                "random",
                ChannelType.Text),
            TestDataFactory.CreateChannel(
                Guid.NewGuid(),
                workspaceId,
                "general",
                ChannelType.Text),
            TestDataFactory.CreateChannel(
                Guid.NewGuid(),
                workspaceId,
                "announcements",
                ChannelType.Text));

        await dbContext.SaveChangesAsync();

        _workspaceAuthorizationMock
            .Setup(x =>
                x.EnsureCanAccessWorkspaceAsync(
                    workspaceId,
                    userId))
            .Returns(Task.CompletedTask);

        var result = await service.GetByWorkspaceIdAsync(
            workspaceId,
            userId);

        var channels = result.ToList();

        Assert.Equal(3, channels.Count);
        Assert.Equal("announcements", channels[0].Name);
        Assert.Equal("general", channels[1].Name);
        Assert.Equal("random", channels[2].Name);

        _workspaceAuthorizationMock.Verify(
            x =>
                x.EnsureCanAccessWorkspaceAsync(
                    workspaceId,
                    userId),
            Times.Once);
    }

    [Fact]
    public async Task GetByWorkspaceIdAsync_WhenUserCannotAccessWorkspace_ThrowsForbiddenException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateChannelService(dbContext);

        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        _workspaceAuthorizationMock
            .Setup(x =>
                x.EnsureCanAccessWorkspaceAsync(
                    workspaceId,
                    userId))
            .ThrowsAsync(
                new ForbiddenException(
                    "User is not a member of this workspace"));

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            service.GetByWorkspaceIdAsync(
                workspaceId,
                userId));

        _workspaceAuthorizationMock.Verify(
            x =>
                x.EnsureCanAccessWorkspaceAsync(
                    workspaceId,
                    userId),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ManageableUserUpdatesChannelName()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            userId,
            WorkspaceRole.Owner);

        var channel = TestDataFactory.CreateChannel(
            channelId,
            workspaceId,
            "old-name",
            ChannelType.Text);

        workspace.Channels.Add(channel);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var service = CreateChannelService(dbContext);

        var request = new UpdateChannelRequestDto
        {
            Name = "  new-name  "
        };

        _workspaceAuthorizationMock
            .Setup(x => x.GetManageableChannelAsync(
                channelId,
                userId))
            .ReturnsAsync(channel);

        var result = await service.UpdateAsync(
            channelId,
            userId,
            request);

        Assert.Equal(channelId, result.Id);
        Assert.Equal(workspaceId, result.WorkspaceId);
        Assert.Equal("new-name", result.Name);

        var savedChannel = await dbContext.Channels
            .FirstAsync(x => x.Id == channelId);

        Assert.Equal("new-name", savedChannel.Name);
    }

    [Fact]
    public async Task UpdateAsync_SameName_ReturnsChannelWithoutChangingIt()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            userId);

        var channel = TestDataFactory.CreateChannel(
            channelId,
            workspaceId,
            "general");

        workspace.Channels.Add(channel);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var service = CreateChannelService(dbContext);

        var request = new UpdateChannelRequestDto
        {
            Name = "  general  "
        };

        _workspaceAuthorizationMock
            .Setup(x => x.GetManageableChannelAsync(
                channelId,
                userId))
            .ReturnsAsync(channel);

        var result = await service.UpdateAsync(
            channelId,
            userId,
            request);

        Assert.Equal("general", result.Name);

        var savedChannel = await dbContext.Channels
            .FirstAsync(x => x.Id == channelId);

        Assert.Equal("general", savedChannel.Name);
    }

    [Fact]
    public async Task DeleteAsync_ManageableUserDeletesChannel()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            userId);

        var channel = TestDataFactory.CreateChannel(
            channelId,
            workspaceId);

        workspace.Channels.Add(channel);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var service = CreateChannelService(dbContext);

        _workspaceAuthorizationMock
            .Setup(x => x.GetManageableChannelAsync(
                channelId,
                userId))
            .ReturnsAsync(channel);

        await service.DeleteAsync(
            channelId,
            userId);

        var deletedChannel = await dbContext.Channels
            .FirstOrDefaultAsync(x => x.Id == channelId);

        Assert.Null(deletedChannel);
    }

    [Fact]
    public async Task DeleteAsync_NonManager_ThrowsForbiddenException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var workspaceId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            ownerId);

        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                memberId,
                WorkspaceRole.Member));

        var channel = TestDataFactory.CreateChannel(
            channelId,
            workspaceId);

        workspace.Channels.Add(channel);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var service = CreateChannelService(dbContext);

        _workspaceAuthorizationMock
            .Setup(x => x.GetManageableChannelAsync(
                channelId,
                memberId))
            .ThrowsAsync(new ForbiddenException("Only workspace administrators can manage channels"));

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            service.DeleteAsync(
                channelId,
                memberId));

        var existingChannel = await dbContext.Channels
            .FirstOrDefaultAsync(x => x.Id == channelId);

        Assert.NotNull(existingChannel);
    }

    private ChannelService CreateChannelService(
        AppDbContext dbContext)
    {
        return new ChannelService(
            dbContext,
            _workspaceAuthorizationMock.Object);
    }
}