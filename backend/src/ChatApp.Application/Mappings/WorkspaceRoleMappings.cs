using ChatApp.Contracts.Workspaces.Enums;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Mappings;

/// <summary>
/// Converts workspace roles between the Domain and Contracts layers
/// </summary>
public static class WorkspaceRoleMappings
{
    /// <summary>
    /// Converts a domain workspace role to its corresponding contract DTO enum value
    /// </summary>
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

    /// <summary>
    /// Converts a contract workspace role DTO enum value to its domain enum value
    /// </summary>
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