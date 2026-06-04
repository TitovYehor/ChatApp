using ChatApp.Application.DTOs.Workspaces;

namespace ChatApp.Application.Interfaces;

public interface IWorkspaceService
{
    Task<WorkspaceResponseDto> CreateAsync(
        Guid userId,
        CreateWorkspaceRequestDto request);

    Task<WorkspaceResponseDto> GetByIdAsync(
        Guid workspaceId,
        Guid userId);
}