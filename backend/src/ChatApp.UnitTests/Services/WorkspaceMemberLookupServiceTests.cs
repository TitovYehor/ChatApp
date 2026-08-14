using ChatApp.Application.Exceptions;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Services;
using ChatApp.UnitTests.Helpers;

namespace ChatApp.UnitTests.Services;

public class WorkspaceMemberLookupServiceTests
{
    [Fact]
    public async Task GetPresenceLookupAsync_WhenUserDoesNotExist_ThrowsNotFoundException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = new WorkspaceMemberLookupService(dbContext);

        var userId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetPresenceLookupAsync(userId));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task GetPresenceLookupAsync_ReturnsUserAndWorkspaceRecipients()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = new WorkspaceMemberLookupService(dbContext);

        var currentUserId = Guid.NewGuid();
        var recipient1Id = Guid.NewGuid();
        var recipient2Id = Guid.NewGuid();
        var unrelatedUserId = Guid.NewGuid();

        var workspace1Id = Guid.NewGuid();
        var workspace2Id = Guid.NewGuid();
        var unrelatedWorkspaceId = Guid.NewGuid();

        var currentUser = TestDataFactory.CreateUser(
            currentUserId,
            "current-user",
            "current@example.com");

        var recipient1 = TestDataFactory.CreateUser(
            recipient1Id,
            "recipient-1",
            "recipient1@example.com");

        var recipient2 = TestDataFactory.CreateUser(
            recipient2Id,
            "recipient-2",
            "recipient2@example.com");

        var unrelatedUser = TestDataFactory.CreateUser(
            unrelatedUserId,
            "unrelated-user",
            "unrelated@example.com");

        dbContext.Users.AddRange(
            currentUser,
            recipient1,
            recipient2,
            unrelatedUser);

        var workspace1 = new Workspace
        {
            Id = workspace1Id,
            Name = "Workspace 1",
            Members =
            [
                TestDataFactory.CreateWorkspaceMember(
                    workspace1Id,
                    currentUserId,
                    WorkspaceRole.Owner),
                TestDataFactory.CreateWorkspaceMember(
                    workspace1Id,
                    recipient1Id,
                    WorkspaceRole.Member)
            ]
        };

        var workspace2 = new Workspace
        {
            Id = workspace2Id,
            Name = "Workspace 2",
            Members =
            [
                TestDataFactory.CreateWorkspaceMember(
                    workspace2Id,
                    currentUserId,
                    WorkspaceRole.Owner),
                TestDataFactory.CreateWorkspaceMember(
                    workspace2Id,
                    recipient1Id,
                    WorkspaceRole.Member),
                TestDataFactory.CreateWorkspaceMember(
                    workspace2Id,
                    recipient2Id,
                    WorkspaceRole.Member)
            ]
        };

        var unrelatedWorkspace = new Workspace
        {
            Id = unrelatedWorkspaceId,
            Name = "Unrelated workspace",
            Members =
            [
                TestDataFactory.CreateWorkspaceMember(
                    unrelatedWorkspaceId,
                    unrelatedUserId,
                    WorkspaceRole.Owner)
            ]
        };

        dbContext.Workspaces.AddRange(
            workspace1,
            workspace2,
            unrelatedWorkspace);

        await dbContext.SaveChangesAsync();

        var result = await service.GetPresenceLookupAsync(
            currentUserId);

        Assert.Equal(currentUserId, result.UserId);
        Assert.Equal("current-user", result.Username);

        Assert.Equal(
            [recipient1Id, recipient2Id],
            result.RecipientUserIds);
    }
}