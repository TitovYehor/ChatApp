using ChatApp.Application.DTOs.Workspaces;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mappings;

public static class WorkspaceMappings
{
    public static WorkspaceResponseDto ToDto(
        this Workspace workspace)
    {
        return new WorkspaceResponseDto
        {
            Id = workspace.Id,
            Name = workspace.Name,
            Description = workspace.Description,
            CreatedAt = workspace.CreatedAt
        };
    }
}