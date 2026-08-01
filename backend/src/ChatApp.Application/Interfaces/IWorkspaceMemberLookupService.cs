using ChatApp.Contracts.Realtime.Responses;
using ChatApp.Contracts.Workspaces.Responses;

namespace ChatApp.Application.Interfaces;

public interface IWorkspaceMemberLookupService
{
    Task<PresenceLookupResponseDto> GetPresenceLookupAsync(
        Guid userId);

    Task<IReadOnlyCollection<OnlineUserResponseDto>> GetOnlineUsersAsync(
        Guid userId,
        IReadOnlyCollection<Guid> onlineUsers);
}