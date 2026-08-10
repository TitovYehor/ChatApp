using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Workspaces.Enums;
using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.Contracts.Workspaces.Responses;
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

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUserWorkspaces()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();

        var workspace1 = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Workspace 1",
            Description = "Description 1"
        };

        var workspace2 = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Workspace 2",
            Description = "Description 2"
        };

        workspace1.Members.Add(
            new WorkspaceMember
            {
                WorkspaceId = workspace1.Id,
                UserId = userId,
                Role = WorkspaceRole.Owner
            });

        workspace2.Members.Add(
            new WorkspaceMember
            {
                WorkspaceId = workspace2.Id,
                UserId = userId,
                Role = WorkspaceRole.Member
            });

        dbContext.Workspaces.AddRange(
            workspace1,
            workspace2);

        await dbContext.SaveChangesAsync();

        var result = await service.GetAllAsync(userId);

        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            x =>
                x.Id == workspace1.Id &&
                x.Name == "Workspace 1");

        Assert.Contains(
            result,
            x =>
                x.Id == workspace2.Id &&
                x.Name == "Workspace 2");
    }

    [Fact]
    public async Task GetAllAsync_ShouldNotReturnWorkspacesUserIsNotMemberOf()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var userWorkspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "User Workspace",
            Description = "User workspace"
        };

        var otherWorkspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Other Workspace",
            Description = "Other workspace"
        };

        userWorkspace.Members.Add(
            new WorkspaceMember
            {
                WorkspaceId = userWorkspace.Id,
                UserId = userId,
                Role = WorkspaceRole.Member
            });

        otherWorkspace.Members.Add(
            new WorkspaceMember
            {
                WorkspaceId = otherWorkspace.Id,
                UserId = otherUserId,
                Role = WorkspaceRole.Owner
            });

        dbContext.Workspaces.AddRange(
            userWorkspace,
            otherWorkspace);

        await dbContext.SaveChangesAsync();

        var result = await service.GetAllAsync(userId);

        var workspace = Assert.Single(result);

        Assert.Equal(
            userWorkspace.Id,
            workspace.Id);

        Assert.Equal(
            "User Workspace",
            workspace.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCorrectWorkspaceRoles()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();

        var ownerWorkspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Owner Workspace"
        };

        var adminWorkspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Admin Workspace"
        };

        var memberWorkspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Member Workspace"
        };

        ownerWorkspace.Members.Add(
            new WorkspaceMember
            {
                WorkspaceId = ownerWorkspace.Id,
                UserId = userId,
                Role = WorkspaceRole.Owner
            });

        adminWorkspace.Members.Add(
            new WorkspaceMember
            {
                WorkspaceId = adminWorkspace.Id,
                UserId = userId,
                Role = WorkspaceRole.Admin
            });

        memberWorkspace.Members.Add(
            new WorkspaceMember
            {
                WorkspaceId = memberWorkspace.Id,
                UserId = userId,
                Role = WorkspaceRole.Member
            });

        dbContext.Workspaces.AddRange(
            ownerWorkspace,
            adminWorkspace,
            memberWorkspace);

        await dbContext.SaveChangesAsync();

        var result = await service.GetAllAsync(userId);

        Assert.Equal(3, result.Count);

        Assert.Equal(
            WorkspaceRoleDto.Owner,
            result.Single(x => x.Id == ownerWorkspace.Id).CurrentUserRole);

        Assert.Equal(
            WorkspaceRoleDto.Admin,
            result.Single(x => x.Id == adminWorkspace.Id).CurrentUserRole);

        Assert.Equal(
            WorkspaceRoleDto.Member,
            result.Single(x => x.Id == memberWorkspace.Id).CurrentUserRole);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenUserHasNoWorkspaces()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();

        var result = await service.GetAllAsync(userId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateWorkspace_WhenUserIsOwner()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var ownerId = Guid.NewGuid();

        var workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Description = "Old Description"
        };

        workspace.Members.Add(
            new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = ownerId,
                Role = WorkspaceRole.Owner
            });

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new UpdateWorkspaceRequestDto
        {
            Name = "  New Name  ",
            Description = "  New Description  "
        };

        var result = await service.UpdateAsync(
            workspace.Id,
            ownerId,
            request);

        Assert.Equal(workspace.Id, result.Id);
        Assert.Equal("New Name", result.Name);
        Assert.Equal("New Description", result.Description);
        Assert.Equal(
            WorkspaceRoleDto.Owner,
            result.CurrentUserRole);

        var updatedWorkspace = await dbContext.Workspaces
            .SingleAsync(x => x.Id == workspace.Id);

        Assert.Equal(
            "New Name",
            updatedWorkspace.Name);

        Assert.Equal(
            "New Description",
            updatedWorkspace.Description);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFound_WhenWorkspaceDoesNotExist()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var request = new UpdateWorkspaceRequestDto
        {
            Name = "New Name",
            Description = "New Description"
        };

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateAsync(
                workspaceId,
                userId,
                request));

        Assert.Equal(
            "Workspace not found",
            exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowForbidden_WhenUserIsNotMember()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var memberId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Description = "Old Description"
        };

        workspace.Members.Add(
            new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = memberId,
                Role = WorkspaceRole.Member
            });

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new UpdateWorkspaceRequestDto
        {
            Name = "New Name",
            Description = "New Description"
        };

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.UpdateAsync(
                workspace.Id,
                otherUserId,
                request));

        Assert.Equal(
            "Not a workspace member",
            exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowForbidden_WhenUserIsMember()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();

        var workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Description = "Old Description"
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

        var request = new UpdateWorkspaceRequestDto
        {
            Name = "New Name",
            Description = "New Description"
        };

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.UpdateAsync(
                workspace.Id,
                userId,
                request));

        Assert.Equal(
            "Only workspace owner can edit workspace",
            exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowForbidden_WhenUserIsAdmin()
    {
        await using var dbContext = CreateDbContext();

        var service = CreateService(dbContext);

        var adminId = Guid.NewGuid();

        var workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Description = "Old Description"
        };

        workspace.Members.Add(
            new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = adminId,
                Role = WorkspaceRole.Admin
            });

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new UpdateWorkspaceRequestDto
        {
            Name = "New Name",
            Description = "New Description"
        };

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.UpdateAsync(
                workspace.Id,
                adminId,
                request));

        Assert.Equal(
            "Only workspace owner can edit workspace",
            exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_OwnerUpdatesWorkspace_AndNotifiesMembers()
    {
        await using var dbContext = CreateDbContext();

        var sut = new WorkspaceService(
            dbContext,
            _workspaceNotifierMock.Object);

        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var workspace = new Workspace
        {
            Id = workspaceId,
            Name = "Old name",
            Description = "Old description",
            Members =
            [
                new WorkspaceMember
                {
                    WorkspaceId = workspaceId,
                    UserId = ownerId,
                    Role = WorkspaceRole.Owner
                },
                new WorkspaceMember
                {
                    WorkspaceId = workspaceId,
                    UserId = memberId,
                    Role = WorkspaceRole.Member
                }
            ]
        };

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new UpdateWorkspaceRequestDto
        {
            Name = "New name",
            Description = "New description"
        };

        var result = await sut.UpdateAsync(
            workspaceId,
            ownerId,
            request);

        Assert.Equal("New name", result.Name);
        Assert.Equal("New description", result.Description);
        Assert.Equal(WorkspaceRoleDto.Owner, result.CurrentUserRole);

        var savedWorkspace = await dbContext.Workspaces
            .FirstAsync(x => x.Id == workspaceId);

        Assert.Equal("New name", savedWorkspace.Name);
        Assert.Equal("New description", savedWorkspace.Description);

        _workspaceNotifierMock.Verify(
            x => x.WorkspaceUpdatedAsync(
                It.Is<IReadOnlyCollection<Guid>>(ids =>
                    ids.Count == 2 &&
                    ids.Contains(ownerId) &&
                    ids.Contains(memberId)),
                It.Is<WorkspaceUpdatedResponseDto>(response =>
                    response.WorkspaceId == workspaceId &&
                    response.Name == "New name" &&
                    response.Description == "New description")),
            Times.Once);
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