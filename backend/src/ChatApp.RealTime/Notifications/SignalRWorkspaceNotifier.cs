using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Realtime.SignalRNamings;
using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.RealTime.Notifications;

/// <summary>
/// Broadcasts workspace and workspace membership events to affected members
/// </summary>
public sealed class SignalRWorkspaceNotifier : IWorkspaceNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRWorkspaceNotifier(
        IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <inheritdoc />
    public async Task WorkspaceDeletedAsync(
        Guid workspaceId,
        IReadOnlyCollection<Guid> memberIds)
    {
        await _hubContext.Clients
            .Users(memberIds.Select(x => x.ToString()))
            .SendAsync(
                SignalREvents.WorkspaceDeleted,
                new WorkspaceDeletedResponseDto
                {
                    WorkspaceId = workspaceId
                });
    }

    /// <inheritdoc />
    public async Task WorkspaceUpdatedAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceUpdatedResponseDto response)
    {
        await _hubContext.Clients
            .Users(memberIds.Select(x => x.ToString()))
            .SendAsync(
                SignalREvents.WorkspaceUpdated,
                response);
    }

    /// <inheritdoc />
    public async Task WorkspaceMemberAddedAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceMemberAddedResponseDto response)
    {
        await _hubContext.Clients
            .Users(memberIds.Select(x => x.ToString()))
            .SendAsync(
                SignalREvents.WorkspaceMemberAdded,
                response);
    }

    /// <inheritdoc />
    public async Task WorkspaceMemberRemovedAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceMemberRemovedResponseDto response)
    {
        await _hubContext.Clients
            .Users(memberIds.Select(x => x.ToString()))
            .SendAsync(
                SignalREvents.WorkspaceMemberRemoved,
                response);
    }

    /// <inheritdoc />
    public async Task WorkspaceMemberRoleChangedAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceMemberRoleChangedResponseDto response)
    {
        await _hubContext.Clients
            .Users(memberIds.Select(x => x.ToString()))
            .SendAsync(
                SignalREvents.WorkspaceMemberRoleChanged,
                response);
    }

    /// <inheritdoc />
    public async Task WorkspaceOwnershipTransferredAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceOwnershipTransferredResponseDto response)
    {
        await _hubContext.Clients
            .Users(memberIds.Select(x => x.ToString()))
            .SendAsync(
                SignalREvents.WorkspaceOwnershipTransferred,
                response);
    }

    /// <inheritdoc />
    public async Task WorkspaceLeftAsync(
        IReadOnlyCollection<Guid> memberIds,
        WorkspaceLeftResponseDto response)
    {
        await _hubContext.Clients
            .Users(memberIds.Select(x => x.ToString()))
            .SendAsync(
                SignalREvents.WorkspaceLeft,
                response);
    }
}