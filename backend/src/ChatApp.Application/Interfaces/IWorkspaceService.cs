using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.Contracts.Workspaces.Responses;

namespace ChatApp.Application.Interfaces;

/// <summary>
/// Defines operations for managing workspaces and their members
/// </summary>
public interface IWorkspaceService
{
    /// <summary>
    /// Creates a workspace owned by the specified user
    /// </summary>
    Task<WorkspaceResponseDto> CreateAsync(
        Guid userId,
        CreateWorkspaceRequestDto request);

    /// <summary>
    /// Retrieves a workspace by its identifier
    /// </summary>
    Task<WorkspaceResponseDto> GetByIdAsync(
        Guid workspaceId,
        Guid userId);

    /// <summary>
    /// Searches for workspaces accessible to the specified user
    /// </summary>
    Task<IReadOnlyCollection<WorkspaceResponseDto>> SearchAsync(
        Guid userId,
        WorkspaceSearchRequestDto request);

    /// <summary>
    /// Retrieves all workspaces accessible to the specified user
    /// </summary>
    Task<IReadOnlyCollection<WorkspaceResponseDto>> GetAllAsync(
        Guid userId);

    /// <summary>
    /// Updates the details of a workspace
    /// </summary>
    Task<WorkspaceResponseDto> UpdateAsync(
        Guid workspaceId,
        Guid userId,
        UpdateWorkspaceRequestDto request);

    /// <summary>
    /// Deletes a workspace
    /// </summary>
    Task DeleteAsync(
        Guid workspaceId,
        Guid currentUserId);

    /// <summary>
    /// Adds a member to a workspace
    /// </summary>
    Task AddMemberAsync(
        Guid workspaceId,
        Guid currentUserId,
        AddWorkspaceMemberRequestDto request);

    /// <summary>
    /// Retrieves the members of a workspace
    /// </summary>
    Task<IReadOnlyCollection<WorkspaceMemberResponseDto>> GetMembersAsync(
        Guid workspaceId,
        Guid userId);

    /// <summary>
    /// Searches for members within a workspace
    /// </summary>
    Task<IReadOnlyCollection<WorkspaceMemberResponseDto>> SearchMembersAsync(
        Guid workspaceId,
        Guid userId,
        WorkspaceMemberSearchRequestDto request);

    /// <summary>
    /// Adds the specified user to a workspace
    /// </summary>
    Task JoinAsync(
        Guid workspaceId,
        Guid userId);

    /// <summary>
    /// Removes the current user from a workspace
    /// </summary>
    Task LeaveAsync(
        Guid workspaceId,
        Guid currentUserId);

    /// <summary>
    /// Removes a member from a workspace
    /// </summary>
    Task RemoveMemberAsync(
        Guid workspaceId,
        Guid currentUserId,
        RemoveWorkspaceMemberRequestDto request);

    /// <summary>
    /// Changes the role of a workspace member
    /// </summary>
    Task ChangeMemberRoleAsync(
        Guid workspaceId,
        Guid currentUserId,
        ChangeWorkspaceMemberRoleRequestDto request);

    /// <summary>
    /// Transfers workspace ownership to another member
    /// </summary>
    Task TransferOwnershipAsync(
        Guid workspaceId,
        Guid currentUserId,
        TransferWorkspaceOwnershipRequestDto request);
}