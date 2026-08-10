using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Workspaces.Enums;
using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ChatApp.UnitTests.Services;

public class WorkspaceServiceTests
{
    private readonly Mock<IWorkspaceNotifier> _workspaceNotifierMock;

    public WorkspaceServiceTests()
    {
        _workspaceNotifierMock = new Mock<IWorkspaceNotifier>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateWorkspaceWithOwnerMembership()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();

        var request = new CreateWorkspaceRequestDto
        {
            Name = "Test Workspace",
            Description = "Test description"
        };

        var result = await service.CreateAsync(
            userId,
            request);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Test Workspace", result.Name);
        Assert.Equal("Test description", result.Description);
        Assert.Equal(WorkspaceRoleDto.Owner, result.CurrentUserRole);

        var workspace = await dbContext.Workspaces
            .FirstOrDefaultAsync(x => x.Id == result.Id);

        Assert.NotNull(workspace);
        Assert.Equal("Test Workspace", workspace.Name);
        Assert.Equal("Test description", workspace.Description);

        var membership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == result.Id &&
                x.UserId == userId);

        Assert.NotNull(membership);
        Assert.Equal(WorkspaceRole.Owner, membership.Role);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnWorkspaceForMember()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();

        var workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Test Workspace",
            Description = "Test description"
        };

        workspace.Members.Add(
            new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = userId,
                Role = WorkspaceRole.Member
            });

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var result = await service.GetByIdAsync(
            workspace.Id,
            userId);

        Assert.Equal(workspace.Id, result.Id);
        Assert.Equal("Test Workspace", result.Name);
        Assert.Equal("Test description", result.Description);
        Assert.Equal(WorkspaceRoleDto.Member, result.CurrentUserRole);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFound_WhenWorkspaceDoesNotExist()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetByIdAsync(
                workspaceId,
                userId));

        Assert.Equal("Workspace not found", exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowForbidden_WhenUserIsNotMember()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var memberUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Test Workspace",
            Description = "Test description"
        };

        workspace.Members.Add(
            new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = memberUserId,
                Role = WorkspaceRole.Member
            });

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.GetByIdAsync(
                workspace.Id,
                otherUserId));

        Assert.Equal("Workspace is forbidden for non members", exception.Message);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private WorkspaceService CreateService(
        AppDbContext dbContext)
    {
        return new WorkspaceService(
            dbContext,
            _workspaceNotifierMock.Object);
    }
}