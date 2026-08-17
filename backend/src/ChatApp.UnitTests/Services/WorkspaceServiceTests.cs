using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Workspaces.Enums;
using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Services;
using ChatApp.UnitTests.Helpers;
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
        await using var dbContext = TestDataFactory.CreateDbContext();

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
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();

        var workspaceId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            userId,
            WorkspaceRole.Member);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var result = await service.GetByIdAsync(
            workspace.Id,
            userId);

        Assert.Equal(workspace.Id, result.Id);
        Assert.Equal("Test workspace", result.Name);
        Assert.Equal("Test description", result.Description);
        Assert.Equal(WorkspaceRoleDto.Member, result.CurrentUserRole);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFound_WhenWorkspaceDoesNotExist()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

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
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var memberUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var workspaceId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            memberUserId,
            WorkspaceRole.Member);

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
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();

        var workspace1Id = Guid.NewGuid();
        var workspace2Id = Guid.NewGuid();

        var workspace1 = TestDataFactory.CreateWorkspace(
            workspace1Id,
            userId,
            WorkspaceRole.Owner);
        workspace1.Name = "Workspace 1";
        workspace1.Description = "Description 1";

        var workspace2 = TestDataFactory.CreateWorkspace(
            workspace2Id,
            userId,
            WorkspaceRole.Member);
        workspace2.Name = "Workspace 2";
        workspace2.Description = "Description 2";

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
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var workspaceId = Guid.NewGuid();
        var otherWorkspaceId = Guid.NewGuid();

        var userWorkspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            userId,
            WorkspaceRole.Member);

        var otherWorkspace = TestDataFactory.CreateWorkspace(
            otherWorkspaceId,
            otherUserId,
            WorkspaceRole.Owner);

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
            "Test workspace",
            workspace.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCorrectWorkspaceRoles()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();
        var ownerWorkspaceId = Guid.NewGuid();
        var adminWorkspaceId = Guid.NewGuid();
        var memberWorkspaceId = Guid.NewGuid();

        var ownerWorkspace = TestDataFactory.CreateWorkspace(
            ownerWorkspaceId,
            userId);

        var adminWorkspace = TestDataFactory.CreateWorkspace(
            adminWorkspaceId,
            userId,
            WorkspaceRole.Admin);

        var memberWorkspace = TestDataFactory.CreateWorkspace(
            memberWorkspaceId,
            userId,
            WorkspaceRole.Member);

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
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();

        var result = await service.GetAllAsync(userId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateWorkspace_WhenUserIsOwner()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var ownerId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            ownerId,
            WorkspaceRole.Owner);

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
        await using var dbContext = TestDataFactory.CreateDbContext();

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
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var memberId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            memberId,
            WorkspaceRole.Member);

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
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            userId,
            WorkspaceRole.Member);

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
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var adminId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            adminId,
            WorkspaceRole.Admin);

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
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                memberId,
                WorkspaceRole.Member)
            );

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new UpdateWorkspaceRequestDto
        {
            Name = "New name",
            Description = "New description"
        };

        var result = await service.UpdateAsync(
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

    [Fact]
    public async Task DeleteAsync_OwnerDeletesWorkspace_AndNotifiesMembers()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var ownerId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                adminId,
                WorkspaceRole.Admin)
            );
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                memberId,
                WorkspaceRole.Member)
            );

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        await service.DeleteAsync(
            workspaceId,
            ownerId);

        var deletedWorkspace = await dbContext.Workspaces
            .FirstOrDefaultAsync(x => x.Id == workspaceId);

        Assert.Null(deletedWorkspace);

        _workspaceNotifierMock.Verify(
            x => x.WorkspaceDeletedAsync(
                workspaceId,
                It.Is<IReadOnlyCollection<Guid>>(ids =>
                    ids.Count == 3 &&
                    ids.Contains(ownerId) &&
                    ids.Contains(adminId) &&
                    ids.Contains(memberId))),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowForbidden_WhenUserIsNotMember()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var ownerId = Guid.NewGuid();
        var nonMemberId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.DeleteAsync(
                workspaceId,
                nonMemberId));

        Assert.Equal(
            "Not a workspace member",
            exception.Message);

        var existingWorkspace = await dbContext.Workspaces
            .FirstOrDefaultAsync(x => x.Id == workspaceId);

        Assert.NotNull(existingWorkspace);

        _workspaceNotifierMock.Verify(
            x => x.WorkspaceDeletedAsync(
                It.IsAny<Guid>(),
                It.IsAny<IReadOnlyCollection<Guid>>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowForbidden_WhenUserIsAdmin()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var adminId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                adminId,
                WorkspaceRole.Admin)
            );

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.DeleteAsync(
                workspaceId,
                adminId));

        Assert.Equal(
            "Only workspace owner can delete workspace",
            exception.Message);

        var existingWorkspace = await dbContext.Workspaces
            .FirstOrDefaultAsync(x => x.Id == workspaceId);

        Assert.NotNull(existingWorkspace);

        _workspaceNotifierMock.Verify(
            x => x.WorkspaceDeletedAsync(
                It.IsAny<Guid>(),
                It.IsAny<IReadOnlyCollection<Guid>>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFound_WhenWorkspaceDoesNotExist()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteAsync(
                workspaceId,
                userId));

        Assert.Equal(
            "Workspace not found",
            exception.Message);

        _workspaceNotifierMock.Verify(
            x => x.WorkspaceDeletedAsync(
                It.IsAny<Guid>(),
                It.IsAny<IReadOnlyCollection<Guid>>()),
            Times.Never);
    }

    [Fact]
    public async Task AddMemberAsync_OwnerAddsUser_ShouldCreateMembership()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var ownerId = Guid.NewGuid();
        var invitedUserId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var owner = TestDataFactory.CreateUser(
            ownerId,
            "owner",
            "owner@test.com");

        var invitedUser = TestDataFactory.CreateUser(
            invitedUserId,
            "newuser",
            "newuser@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);

        dbContext.Users.AddRange(owner, invitedUser);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new AddWorkspaceMemberRequestDto
        {
            UsernameOrEmail = invitedUser.Username
        };

        await service.AddMemberAsync(
            workspaceId,
            ownerId,
            request);

        var membership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == invitedUserId);

        Assert.NotNull(membership);
        Assert.Equal(WorkspaceRole.Member, membership.Role);
    }

    [Fact]
    public async Task AddMemberAsync_AdminAddsUser_ShouldCreateMembership()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var adminId = Guid.NewGuid();
        var invitedUserId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var admin = TestDataFactory.CreateUser(
            adminId,
            "admin",
            "admin@test.com");

        var invitedUser = TestDataFactory.CreateUser(
            invitedUserId,
            "newuser",
            "newuser@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            adminId, 
            WorkspaceRole.Admin);

        dbContext.Users.AddRange(admin, invitedUser);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new AddWorkspaceMemberRequestDto
        {
            UsernameOrEmail = invitedUser.Email
        };

        await service.AddMemberAsync(
            workspaceId,
            adminId,
            request);

        var membership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == invitedUserId);

        Assert.NotNull(membership);
        Assert.Equal(WorkspaceRole.Member, membership.Role);
    }

    [Fact]
    public async Task AddMemberAsync_NonMember_ShouldThrowForbidden()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var ownerId = Guid.NewGuid();
        var nonMemberId = Guid.NewGuid();
        var invitedUserId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var invitedUser = TestDataFactory.CreateUser(
            invitedUserId,
            "newuser",
            "newuser@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);

        dbContext.Users.Add(invitedUser);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new AddWorkspaceMemberRequestDto
        {
            UsernameOrEmail = invitedUser.Username
        };

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.AddMemberAsync(
                workspaceId,
                nonMemberId,
                request));

        Assert.Equal(
            "Inviting user are not a member of this workspace",
            exception.Message);
    }

    [Fact]
    public async Task AddMemberAsync_Member_ShouldThrowForbidden()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var memberId = Guid.NewGuid();
        var invitedUserId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var service = CreateService(dbContext);

        var invitedUser = TestDataFactory.CreateUser(
            invitedUserId,
            "newuser",
            "newuser@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            memberId,
            WorkspaceRole.Member);

        dbContext.Users.Add(invitedUser);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new AddWorkspaceMemberRequestDto
        {
            UsernameOrEmail = invitedUser.Username
        };

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.AddMemberAsync(
                workspaceId,
                memberId,
                request));

        Assert.Equal(
            "Users with 'Member' role are not allowed to invite",
            exception.Message);
    }

    [Fact]
    public async Task AddMemberAsync_UserDoesNotExist_ShouldThrowNotFound()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var ownerId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new AddWorkspaceMemberRequestDto
        {
            UsernameOrEmail = "does-not-exist"
        };

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.AddMemberAsync(
                workspaceId,
                ownerId,
                request));

        Assert.Equal(
            "Invited user not found",
            exception.Message);
    }

    [Fact]
    public async Task AddMemberAsync_UserAlreadyMember_ShouldThrowConflict()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var ownerId = Guid.NewGuid();
        var existingMemberId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var service = CreateService(dbContext);

        var owner = TestDataFactory.CreateUser(
            ownerId,    
            "owner",
            "owner@test.com");

        var existingMember = TestDataFactory.CreateUser(
            existingMemberId,
            "existing",
            "existing@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                existingMemberId,
                WorkspaceRole.Member)
            );

        dbContext.Users.AddRange(owner, existingMember);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new AddWorkspaceMemberRequestDto
        {
            UsernameOrEmail = existingMember.Username
        };

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.AddMemberAsync(
                workspaceId,
                ownerId,
                request));

        Assert.Equal(
            "User is already a workspace member",
            exception.Message);

        var memberships = await dbContext.WorkspaceMembers
            .Where(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == existingMemberId)
            .ToListAsync();

        Assert.Single(memberships);
    }

    [Fact]
    public async Task AddMemberAsync_WorkspaceDoesNotExist_ShouldThrowNotFound()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var request = new AddWorkspaceMemberRequestDto
        {
            UsernameOrEmail = "someone"
        };

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.AddMemberAsync(
                workspaceId,
                userId,
                request));

        Assert.Equal(
            "Workspace not found",
            exception.Message);
    }

    [Fact]
    public async Task GetMembersAsync_MemberGetsWorkspaceMembers_OrderedByRoleThenUsername()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();

        var ownerId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var memberZId = Guid.NewGuid();
        var memberAId = Guid.NewGuid();

        var owner = TestDataFactory.CreateUser(
            ownerId,
            "owner",
            "owner@test.com");

        var admin = TestDataFactory.CreateUser(
            adminId,
            "admin",
            "admin@test.com");

        var currentUser = TestDataFactory.CreateUser(
            currentUserId,
            "currentuser",
            "currentuser@test.com");

        var memberA = TestDataFactory.CreateUser(
            memberAId,
            "auser",
            "auser@test.com");

        var memberZ = TestDataFactory.CreateUser(
            memberZId,
            "zuser",
            "zuser@test.com");

        dbContext.Users.AddRange(
            owner,
            admin,
            currentUser,
            memberA,
            memberZ);

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                adminId,
                WorkspaceRole.Admin)
            );
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                memberAId,
                WorkspaceRole.Member)
            );
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                memberZId,
                WorkspaceRole.Member)
            );
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                currentUserId,
                WorkspaceRole.Member)
            );

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var result = await service.GetMembersAsync(
            workspaceId,
            currentUserId);

        Assert.Equal(5, result.Count);

        Assert.Equal(ownerId, result.ElementAt(0).UserId);
        Assert.Equal(WorkspaceRoleDto.Owner, result.ElementAt(0).Role);

        Assert.Equal(adminId, result.ElementAt(1).UserId);
        Assert.Equal(WorkspaceRoleDto.Admin, result.ElementAt(1).Role);

        Assert.Equal(memberAId, result.ElementAt(2).UserId);
        Assert.Equal(WorkspaceRoleDto.Member, result.ElementAt(2).Role);

        Assert.Equal(currentUserId, result.ElementAt(3).UserId);
        Assert.Equal(WorkspaceRoleDto.Member, result.ElementAt(3).Role);

        Assert.Equal(memberZId, result.ElementAt(4).UserId);
        Assert.Equal(WorkspaceRoleDto.Member, result.ElementAt(4).Role);
    }

    [Fact]
    public async Task GetMembersAsync_NonMember_ThrowsForbiddenException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var nonMemberId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            memberId);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            service.GetMembersAsync(
                workspaceId,
                nonMemberId));
    }

    [Fact]
    public async Task JoinAsync_UserJoinsWorkspace_AddsMember()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var newUserId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            userId);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        await service.JoinAsync(
            workspaceId,
            newUserId);

        var membership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == newUserId);

        Assert.NotNull(membership);
        Assert.Equal(WorkspaceRole.Member, membership.Role);
    }

    [Fact]
    public async Task JoinAsync_WorkspaceDoesNotExist_ThrowsNotFoundException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.JoinAsync(
                workspaceId,
                userId));

        Assert.Equal(
            "Workspace not found",
            exception.Message);
    }

    [Fact]
    public async Task JoinAsync_UserAlreadyMember_ThrowsConflictException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            userId);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.JoinAsync(
                workspaceId,
                userId));

        Assert.Equal(
            "User is already a member of this workspace",
            exception.Message);

        var memberCount = await dbContext.WorkspaceMembers
            .CountAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == userId);

        Assert.Equal(1, memberCount);
    }

    [Fact]
    public async Task LeaveAsync_MemberLeavesWorkspace_RemovesMembership()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                memberId,
                WorkspaceRole.Member)
            );

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        await service.LeaveAsync(
            workspaceId,
            memberId);

        var membership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == memberId);

        Assert.Null(membership);

        var ownerMembership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == ownerId);

        Assert.NotNull(ownerMembership);
        Assert.Equal(WorkspaceRole.Owner, ownerMembership.Role);
    }

    [Fact]
    public async Task LeaveAsync_OwnerLeavesWorkspace_ThrowsConflictException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);

        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.LeaveAsync(
                workspaceId,
                ownerId));

        Assert.Equal(
            "Transfer workspace ownership before leaving the workspace",
            exception.Message);

        var membership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == ownerId);

        Assert.NotNull(membership);
        Assert.Equal(WorkspaceRole.Owner, membership.Role);
    }

    [Fact]
    public async Task RemoveMemberAsync_AdminRemovesMember_ShouldRemoveMembership()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        var admin = TestDataFactory.CreateUser(
            adminId,
            "admin",
            "admin@test.com");

        var member = TestDataFactory.CreateUser(
            memberId,
            "member",
            "member@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            adminId,
            WorkspaceRole.Admin);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                memberId,
                WorkspaceRole.Member)
            );

        dbContext.Users.AddRange(admin, member);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new RemoveWorkspaceMemberRequestDto
        {
            UsernameOrEmail = member.Username
        };

        await service.RemoveMemberAsync(
            workspaceId,
            adminId,
            request);

        var membership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == memberId);

        Assert.Null(membership);

        var adminMembership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == adminId);

        Assert.NotNull(adminMembership);
        Assert.Equal(WorkspaceRole.Admin, adminMembership.Role);
    }

    [Fact]
    public async Task RemoveMemberAsync_MemberTriesToRemoveUser_ShouldThrowForbidden()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var targetUser = TestDataFactory.CreateUser(
            targetUserId,
            "target",
            "target@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            currentUserId,
            WorkspaceRole.Member);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                targetUserId,
                WorkspaceRole.Member)
            );

        dbContext.Users.Add(targetUser);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new RemoveWorkspaceMemberRequestDto
        {
            UsernameOrEmail = targetUser.Username
        };

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.RemoveMemberAsync(
                workspaceId,
                currentUserId,
                request));

        Assert.Equal(
            "Only Owner or Admin can remove members",
            exception.Message);

        var membership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == targetUserId);

        Assert.NotNull(membership);
    }

    [Fact]
    public async Task RemoveMemberAsync_TargetIsOwner_ShouldThrowConflict()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var owner = TestDataFactory.CreateUser(
            ownerId,
            "owner",
            "owner@test.com");

        var admin = TestDataFactory.CreateUser(
            adminId,
            "admin",
            "admin@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                adminId,
                WorkspaceRole.Admin)
            );

        dbContext.Users.AddRange(owner, admin);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new RemoveWorkspaceMemberRequestDto
        {
            UsernameOrEmail = owner.Username
        };

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.RemoveMemberAsync(
                workspaceId,
                adminId,
                request));

        Assert.Equal(
            "Workspace owner cannot be removed",
            exception.Message);

        var membership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == ownerId);

        Assert.NotNull(membership);
        Assert.Equal(WorkspaceRole.Owner, membership.Role);
    }

    [Fact]
    public async Task ChangeMemberRoleAsync_OwnerChangesMemberRole_ShouldUpdateRole()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        var owner = TestDataFactory.CreateUser(
            ownerId,
            "owner",
            "owner@test.com");

        var member = TestDataFactory.CreateUser(
            memberId,
            "member",
            "member@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                memberId,
                WorkspaceRole.Member)
            );

        dbContext.Users.AddRange(owner, member);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new ChangeWorkspaceMemberRoleRequestDto
        {
            UsernameOrEmail = member.Username,
            Role = WorkspaceRoleDto.Admin
        };

        await service.ChangeMemberRoleAsync(
            workspaceId,
            ownerId,
            request);

        var membership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == memberId);

        Assert.NotNull(membership);
        Assert.Equal(WorkspaceRole.Admin, membership.Role);
    }

    [Fact]
    public async Task ChangeMemberRoleAsync_NonOwnerTriesToChangeRole_ShouldThrowForbidden()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        var admin = TestDataFactory.CreateUser(
            adminId,
            "admin",
            "admin@test.com");

        var member = TestDataFactory.CreateUser(
            memberId,
            "member",
            "member@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            adminId,
            WorkspaceRole.Admin);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                memberId,
                WorkspaceRole.Member)
            );

        dbContext.Users.AddRange(admin, member);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new ChangeWorkspaceMemberRoleRequestDto
        {
            UsernameOrEmail = member.Username,
            Role = WorkspaceRoleDto.Admin
        };

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.ChangeMemberRoleAsync(
                workspaceId,
                adminId,
                request));

        Assert.Equal(
            "Only owner can change roles",
            exception.Message);

        var membership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == memberId);

        Assert.NotNull(membership);
        Assert.Equal(WorkspaceRole.Member, membership.Role);
    }

    [Fact]
    public async Task ChangeMemberRoleAsync_OwnerChangesOwnRole_ShouldThrowConflict()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var workspaceId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var owner = TestDataFactory.CreateUser(
            ownerId,
            "owner",
            "owner@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);

        dbContext.Users.Add(owner);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new ChangeWorkspaceMemberRoleRequestDto
        {
            UsernameOrEmail = owner.Username,
            Role = WorkspaceRoleDto.Admin
        };

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => service.ChangeMemberRoleAsync(
                workspaceId,
                ownerId,
                request));

        Assert.Equal(
            "Owner cannot change own role",
            exception.Message);

        var membership = await dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == ownerId);

        Assert.NotNull(membership);
        Assert.Equal(WorkspaceRole.Owner, membership.Role);
    }

    [Fact]
    public async Task TransferOwnershipAsync_OwnerTransfersOwnership_SavesNewRoles()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var owner = TestDataFactory.CreateUser(
            ownerId,
            "owner",
            "owner@test.com");

        var member = TestDataFactory.CreateUser(
            memberId,
            "member",
            "member@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId,
                memberId,
                WorkspaceRole.Member)
            );

        dbContext.Users.AddRange(owner, member);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new TransferWorkspaceOwnershipRequestDto
        {
            UsernameOrEmail = member.Username
        };

        await service.TransferOwnershipAsync(
            workspaceId,
            ownerId,
            request);

        var memberships = await dbContext.WorkspaceMembers
            .Where(x => x.WorkspaceId == workspaceId)
            .ToListAsync();

        var savedOwner = memberships
            .First(x => x.UserId == ownerId);

        var savedNewOwner = memberships
            .First(x => x.UserId == memberId);

        Assert.Equal(
            WorkspaceRole.Admin,
            savedOwner.Role);

        Assert.Equal(
            WorkspaceRole.Owner,
            savedNewOwner.Role);
    }

    [Fact]
    public async Task TransferOwnershipAsync_NonOwner_ThrowsForbiddenException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var owner = TestDataFactory.CreateUser(
            ownerId,
            "owner",
            "owner@test.com");

        var member = TestDataFactory.CreateUser(
            memberId,
            "member",
            "member@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId,
            ownerId);
        workspace.Members.Add(
            TestDataFactory.CreateWorkspaceMember(
                workspaceId, 
                memberId, 
                WorkspaceRole.Member)
            );

        dbContext.Users.AddRange(owner, member);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new TransferWorkspaceOwnershipRequestDto
        {
            UsernameOrEmail = owner.Username
        };

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => service.TransferOwnershipAsync(
                workspaceId,
                memberId,
                request));

        Assert.Equal(
            "Only workspace owner can transfer ownership",
            exception.Message);
    }

    [Fact]
    public async Task TransferOwnershipAsync_TargetIsNotMember_ThrowsNotFoundException()
    {
        await using var dbContext = TestDataFactory.CreateDbContext();

        var service = CreateService(dbContext);

        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        var owner = TestDataFactory.CreateUser(
            ownerId,
            "owner",
            "owner@test.com");

        var otherUser = TestDataFactory.CreateUser(
            otherUserId, 
            "outsider", 
            "outsider@test.com");

        var workspace = TestDataFactory.CreateWorkspace(
            workspaceId, 
            ownerId);

        dbContext.Users.AddRange(owner, otherUser);
        dbContext.Workspaces.Add(workspace);

        await dbContext.SaveChangesAsync();

        var request = new TransferWorkspaceOwnershipRequestDto
        {
            UsernameOrEmail = otherUser.Username
        };

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => service.TransferOwnershipAsync(
                workspaceId,
                ownerId,
                request));

        Assert.Equal(
            "User not found",
            exception.Message);
    }

    private WorkspaceService CreateService(
        AppDbContext dbContext)
    {
        return new WorkspaceService(
            dbContext,
            _workspaceNotifierMock.Object);
    }
}