using ChatApp.Contracts.Workspaces.Enums;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Mappings;

public static class WorkspaceRoleMappings
{
    public static WorkspaceRoleDto ToDto(
        this WorkspaceRole role)
    {
        return role switch
        {
            WorkspaceRole.Owner => WorkspaceRoleDto.Owner,
            WorkspaceRole.Admin => WorkspaceRoleDto.Admin,
            WorkspaceRole.Member => WorkspaceRoleDto.Member,
            _ => throw new ArgumentOutOfRangeException(
                nameof(role),
                role,
                "Unknown workspace role")
        };
    }

    public static WorkspaceRole ToDomain(
        this WorkspaceRoleDto role)
    {
        return role switch
        {
            WorkspaceRoleDto.Owner => WorkspaceRole.Owner,
            WorkspaceRoleDto.Admin => WorkspaceRole.Admin,
            WorkspaceRoleDto.Member => WorkspaceRole.Member,
            _ => throw new ArgumentOutOfRangeException(
                nameof(role),
                role,
                "Unknown workspace role")
        };
    }
}