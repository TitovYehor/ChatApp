using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.SignalRTester.Models;

namespace ChatApp.SignalRTester.Clients.Workspaces;

public interface IWorkspaceApiClient
{
    Task<ApiResult<WorkspaceResponseDto>> CreateAsync(
        CreateWorkspaceRequestDto request);

    Task<ApiResult<IReadOnlyList<WorkspaceResponseDto>>> GetAllAsync();

    Task<ApiResult<WorkspaceResponseDto>> GetByIdAsync(
        Guid workspaceId);

    Task<ApiResult<bool>> AddMemberAsync(
        Guid workspaceId,
        AddWorkspaceMemberRequestDto request);

    Task<ApiResult<IReadOnlyList<WorkspaceMemberResponseDto>>> GetMembersAsync(
        Guid workspaceId);

    Task<ApiResult<bool>> JoinAsync(
        Guid workspaceId);

    Task<ApiResult<bool>> LeaveAsync(
        Guid workspaceId);

    Task<ApiResult<bool>> RemoveMemberAsync(
        Guid workspaceId,
        RemoveWorkspaceMemberRequestDto request);

    Task<ApiResult<bool>> ChangeMemberRoleAsync(
        Guid workspaceId,
        ChangeWorkspaceMemberRoleRequestDto request);

    Task<ApiResult<bool>> TransferOwnershipAsync(
        Guid workspaceId,
        TransferWorkspaceOwnershipRequestDto request);
}