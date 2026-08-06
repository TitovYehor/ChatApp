using ChatApp.Application.Interfaces;
using ChatApp.Contracts.Realtime.SignalRNamings;
using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.RealTime.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.RealTime.Services;

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
}