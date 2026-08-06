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
}