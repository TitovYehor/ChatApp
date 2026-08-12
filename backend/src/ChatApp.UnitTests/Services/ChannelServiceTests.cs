using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Channels.Requests;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ChatApp.UnitTests.Services;

public class ChannelServiceTests
{
    private readonly Mock<IWorkspaceAuthorizationService>
        _workspaceAuthorizationMock = new();

    [Fact]
    public async Task CreateAsync_AuthorizedUser_CreatesChannel()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateChannelService(dbContext);

        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var workspace = CreateWorkspace(
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
        await using var dbContext = CreateDbContext();

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

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private ChannelService CreateChannelService(
        AppDbContext dbContext)
    {
        return new ChannelService(
            dbContext,
            _workspaceAuthorizationMock.Object);
    }

    private static Workspace CreateWorkspace(
        Guid workspaceId,
        Guid userId,
        WorkspaceRole role = WorkspaceRole.Owner)
    {
        return new Workspace
        {
            Id = workspaceId,
            Name = "Test workspace",
            Description = "Test description",
            Members =
            [
                new WorkspaceMember
                {
                    WorkspaceId = workspaceId,
                    UserId = userId,
                    Role = role
                }
            ]
        };
    }
}