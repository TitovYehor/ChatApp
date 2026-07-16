using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mappings;
using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services;

public class WorkspaceService : IWorkspaceService
{
    private readonly AppDbContext _dbContext;

    public WorkspaceService(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WorkspaceResponseDto> CreateAsync(
        Guid userId,
        CreateWorkspaceRequestDto request)
    {
        var workspace = new Workspace
        {
            Name = request.Name,
            Description = request.Description
        };

        var membership = new WorkspaceMember
        {
            Workspace = workspace,
            UserId = userId,
            Role = WorkspaceRole.Owner
        };

        _dbContext.Workspaces.Add(workspace);

        _dbContext.WorkspaceMembers.Add(membership);

        await _dbContext.SaveChangesAsync();

        return workspace.ToDto();
    }

    public async Task<WorkspaceResponseDto> GetByIdAsync(
        Guid workspaceId,
        Guid userId)
    {
        var isMember = await _dbContext.WorkspaceMembers
            .AnyAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == userId);

        if (!isMember)
        {
            throw new ForbiddenException("Workspace is forbidden for non members");
        }

        var workspace = await _dbContext.Workspaces
            .FirstOrDefaultAsync(x =>
                x.Id == workspaceId);

        if (workspace == null)
        {
            throw new NotFoundException("Workspace not found");
        }

        return workspace.ToDto();
    }

    public async Task<IReadOnlyCollection<WorkspaceResponseDto>> GetAllAsync(
        Guid userId)
    {
        var workspaces = await _dbContext.Workspaces
            .AsNoTracking()
            .Where(x =>
                x.Members.Any(m => m.UserId == userId))
            .ToListAsync();

        return workspaces
            .Select(x => x.ToDto())
            .ToList();
    }

    public async Task AddMemberAsync(
        Guid workspaceId,
        Guid currentUserId,
        AddWorkspaceMemberRequestDto request)
    {
        var workspace = await _dbContext.Workspaces
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x =>
                x.Id == workspaceId);

        if (workspace == null)
        {
            throw new NotFoundException("Workspace not found");
        }

        var currentMembership = workspace.Members
            .FirstOrDefault(x =>
                x.UserId == currentUserId);

        if (currentMembership == null)
        { 
            throw new ForbiddenException("Inviting user are not a member of this workspace");
        }
        if (currentMembership.Role == WorkspaceRole.Member)
        {
            throw new ForbiddenException("Users with 'Member' role are not allowed to invite");
        }

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x =>
                x.Email == request.UsernameOrEmail ||
                x.Username == request.UsernameOrEmail);

        if (user == null)
        {
            throw new NotFoundException("Invited user not found");
        }
        if (workspace.Members.Any(x => x.UserId == user.Id))
        {
            throw new ConflictException("User is already a workspace member");
        }

        _dbContext.WorkspaceMembers.Add(
            new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = user.Id,
                Role = WorkspaceRole.Member
            });

        await _dbContext.SaveChangesAsync();
    }

    public async Task JoinAsync(
        Guid workspaceId,
        Guid userId)
    {
        var workspaceExists = await _dbContext.Workspaces
            .AnyAsync(x =>
                x.Id == workspaceId);

        if (!workspaceExists)
        {
            throw new NotFoundException("Workspace not found");
        }

        var alreadyMember = await _dbContext.WorkspaceMembers
            .AnyAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == userId);

        if (alreadyMember)
        {
            throw new ConflictException("User is already a member of this workspace");
        }

        var membership = new WorkspaceMember
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            Role = WorkspaceRole.Member
        };

        _dbContext.WorkspaceMembers.Add(
            membership);

        await _dbContext.SaveChangesAsync();
    }
}