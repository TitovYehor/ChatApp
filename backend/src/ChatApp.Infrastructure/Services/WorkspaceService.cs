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
}