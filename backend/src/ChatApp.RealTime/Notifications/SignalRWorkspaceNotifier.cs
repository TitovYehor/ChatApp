using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Realtime.SignalRNamings;
using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.RealTime.Notifications;

public sealed class SignalRWorkspaceNotifier : IWorkspaceNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRWorkspaceNotifier(
        IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

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
}