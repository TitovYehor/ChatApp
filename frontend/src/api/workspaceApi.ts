import { apiRequest } from './client'
import type {
    AddWorkspaceMemberRequest,
    ChangeWorkspaceMemberRoleRequest,
    CreateWorkspaceRequest,
    RemoveWorkspaceMemberRequest,
    TransferWorkspaceOwnershipRequest,
    UpdateWorkspaceRequest,
    WorkspaceMemberResponse,
    WorkspaceResponse,
    WorkspaceSearchRequest,
    WorkspaceMemberSearchRequest
} from '../types/workspaceTypes'

export function getAll():
    Promise<WorkspaceResponse[]> {
    return apiRequest<WorkspaceResponse[]>(
        '/workspaces',
    )
}

export function search(
    request: WorkspaceSearchRequest,
): Promise<WorkspaceResponse[]> {
    const params = new URLSearchParams()

    if (request.query) {
        params.set(
            'query',
            request.query,
        )
    }

    return apiRequest<WorkspaceResponse[]>(
        `/workspaces/search?${params.toString()}`,
    )
}

export function getById(
    workspaceId: string,
): Promise<WorkspaceResponse> {
    return apiRequest<WorkspaceResponse>(
        `/workspaces/${workspaceId}`,
    )
}

export function create(
    request: CreateWorkspaceRequest,
): Promise<WorkspaceResponse> {
    return apiRequest<WorkspaceResponse>(
        '/workspaces',
        {
            method: 'POST',
            body: JSON.stringify(request),
        },
    )
}

export function update(
    workspaceId: string,
    request: UpdateWorkspaceRequest,
): Promise<WorkspaceResponse> {
    return apiRequest<WorkspaceResponse>(
        `/workspaces/${workspaceId}`,
        {
            method: 'PUT',
            body: JSON.stringify(request),
        },
    )
}

export function remove(
    workspaceId: string,
): Promise<void> {
    return apiRequest<void>(
        `/workspaces/${workspaceId}`,
        {
            method: 'DELETE',
        },
    )
}

export function addMember(
    workspaceId: string,
    request: AddWorkspaceMemberRequest,
): Promise<void> {
    return apiRequest<void>(
        `/workspaces/${workspaceId}/members`,
        {
            method: 'POST',
            body: JSON.stringify(request),
        },
    )
}

export function getMembers(
    workspaceId: string,
): Promise<WorkspaceMemberResponse[]> {
    return apiRequest<WorkspaceMemberResponse[]>(
        `/workspaces/${workspaceId}/members`,
    )
}

export function searchMembers(
    workspaceId: string,
    request: WorkspaceMemberSearchRequest,
): Promise<WorkspaceMemberResponse[]> {
    const params = new URLSearchParams()

    if (request.query) {
        params.set('query', request.query)
    }

    return apiRequest<WorkspaceMemberResponse[]>(
        `/workspaces/${workspaceId}/members/search?${params.toString()}`,
    )
}

export function leave(
    workspaceId: string,
): Promise<void> {
    return apiRequest<void>(
        `/workspaces/${workspaceId}/leave`,
        {
            method: 'POST',
        },
    )
}

export function removeMember(
    workspaceId: string,
    request: RemoveWorkspaceMemberRequest,
): Promise<void> {
    return apiRequest<void>(
        `/workspaces/${workspaceId}/members`,
        {
            method: 'DELETE',
            body: JSON.stringify(request),
        },
    )
}

export function changeMemberRole(
    workspaceId: string,
    request: ChangeWorkspaceMemberRoleRequest,
): Promise<void> {
    return apiRequest<void>(
        `/workspaces/${workspaceId}/members/role`,
        {
            method: 'PUT',
            body: JSON.stringify(request),
        },
    )
}

export function transferOwnership(
    workspaceId: string,
    request: TransferWorkspaceOwnershipRequest,
): Promise<void> {
    return apiRequest<void>(
        `/workspaces/${workspaceId}/ownership`,
        {
            method: 'PUT',
            body: JSON.stringify(request),
        },
    )
}