using ChatApp.Contracts.Workspaces.Responses;

namespace ChatApp.Application.Interfaces;

public interface IWorkspaceNotifier
{
    Task WorkspaceDeletedAsync(
        Guid workspaceId,
        IReadOnlyCollection<Guid> memberIds);

    Task WorkspaceUpdatedAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceUpdatedResponseDto response);

    Task WorkspaceMemberAddedAsync(
        Guid userId,
        WorkspaceMemberAddedResponseDto response);

    Task WorkspaceMemberRemovedAsync(
        Guid userId,
        WorkspaceMemberRemovedResponseDto response);

    Task WorkspaceMemberRoleChangedAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceMemberRoleChangedResponseDto response);

    Task WorkspaceOwnershipTransferredAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceOwnershipTransferredResponseDto response);
}