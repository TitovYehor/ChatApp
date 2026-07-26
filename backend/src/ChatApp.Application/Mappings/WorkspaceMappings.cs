using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Mappings;

public static class WorkspaceMappings
{
    public static WorkspaceResponseDto ToDto(
        this Workspace workspace,
        WorkspaceRole role)
    {
        return new WorkspaceResponseDto
        {
            Id = workspace.Id,
            Name = workspace.Name,
            Description = workspace.Description,
            CreatedAt = workspace.CreatedAt,
            CurrentUserRole = role.ToDto()
        };
    }
}