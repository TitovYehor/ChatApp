using ChatApp.Application.DTOs.Workspaces;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mappings;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Persistence;

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
}