using ChatApp.Contracts.Workspaces.Requests;
using ChatApp.Contracts.Workspaces.Responses;

namespace ChatApp.Application.Interfaces;

public interface IWorkspaceService
{
    Task<WorkspaceResponseDto> CreateAsync(
        Guid userId,
        CreateWorkspaceRequestDto request);

    Task<WorkspaceResponseDto> GetByIdAsync(
        Guid workspaceId,
        Guid userId);

    Task<IReadOnlyCollection<WorkspaceResponseDto>> GetAllAsync(
        Guid userId);
}