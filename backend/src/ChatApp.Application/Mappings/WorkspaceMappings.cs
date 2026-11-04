using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Mappings;

/// <summary>
/// Defines mappings between workspace domain entities and response DTOs
/// </summary>
public static class WorkspaceMappings
{
    /// <summary>
    /// Maps a workspace entity and the current user's role to a response DTO
    /// </summary>
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