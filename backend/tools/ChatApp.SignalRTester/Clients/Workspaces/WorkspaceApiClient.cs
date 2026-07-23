using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.Contracts.Workspaces.Responses;
using ChatApp.SignalRTester.Configuration;
using ChatApp.SignalRTester.Models;
using ChatApp.SignalRTester.Session;
using Microsoft.Extensions.Options;

namespace ChatApp.SignalRTester.Clients.Workspaces;

public class WorkspaceApiClient : ApiClientBase, IWorkspaceApiClient
{
    public WorkspaceApiClient(
        IHttpClientFactory factory,
        IOptions<AppSettings> options,
        UserSession session)
        : base(factory.CreateClient(), session)
    {
        HttpClient.BaseAddress = new Uri(options.Value.ApiBaseUrl);
    }

    public Task<ApiResult<WorkspaceResponseDto>> CreateAsync(
        CreateWorkspaceRequestDto request)
    {
        return PostAsync<CreateWorkspaceRequestDto, WorkspaceResponseDto>(
            "api/workspaces",
            request);
    }

    public Task<ApiResult<IReadOnlyList<WorkspaceResponseDto>>> GetAllAsync()
    {
        return GetAsync<IReadOnlyList<WorkspaceResponseDto>>(
            "api/workspaces");
    }

    public Task<ApiResult<WorkspaceResponseDto>> GetByIdAsync(
        Guid workspaceId)
    {
        return GetAsync<WorkspaceResponseDto>(
            $"api/workspaces/{workspaceId}");
    }

    public Task<ApiResult<bool>> AddMemberAsync(
        Guid workspaceId,
        AddWorkspaceMemberRequestDto request)
    {
        return PostEmptyAsync(
            $"api/workspaces/{workspaceId}/members",
            request);
    }

    public Task<ApiResult<IReadOnlyList<WorkspaceMemberResponseDto>>> GetMembersAsync(
        Guid workspaceId)
    {
        return GetAsync<IReadOnlyList<WorkspaceMemberResponseDto>>(
            $"api/workspaces/{workspaceId}/members");
    }

    public async Task<ApiResult<bool>> JoinAsync(
        Guid workspaceId)
    {
        return await PostEmptyAsync(
            $"api/workspaces/{workspaceId}/join");
    }

    public Task<ApiResult<bool>> LeaveAsync(
        Guid workspaceId)
    {
        return PostEmptyAsync(
            $"api/workspaces/{workspaceId}/leave");
    }

    public Task<ApiResult<bool>> RemoveMemberAsync(
        Guid workspaceId,
        RemoveWorkspaceMemberRequestDto request)
    {
        return DeleteAsync(
            $"api/workspaces/{workspaceId}/members",
            request);
    }

    public Task<ApiResult<bool>> ChangeMemberRoleAsync(
        Guid workspaceId,
        ChangeWorkspaceMemberRoleRequestDto request)
    {
        return PutWithoutResponseAsync(
            $"api/workspaces/{workspaceId}/members/role",
            request);
    }
}