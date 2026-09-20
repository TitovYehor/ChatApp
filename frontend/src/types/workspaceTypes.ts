export interface CreateWorkspaceRequest {
    name: string
    description: string
}

export interface WorkspaceResponse {
    id: string
    name: string
    description: string
    createdAt: string
    currentUserRole: WorkspaceRole | null
}

export interface UpdateWorkspaceRequest {
    name: string
    description: string
}

export interface AddWorkspaceMemberRequest {
    usernameOrEmail: string
}

export interface RemoveWorkspaceMemberRequest {
    usernameOrEmail: string
}

export interface ChangeWorkspaceMemberRoleRequest {
    usernameOrEmail: string
    role: WorkspaceRole
}

export interface TransferWorkspaceOwnershipRequest {
    usernameOrEmail: string
}

export interface WorkspaceMemberResponse {
    userId: string
    username: string
    email: string
    role: WorkspaceRole
    joinedAt: string
}

export interface WorkspaceUpdatedResponse {
    workspaceId: string
    name: string
    description: string
}

export interface WorkspaceDeletedResponse {
    workspaceId: string
}

export interface WorkspaceMemberAddedResponse {
    workspaceId: string
    name: string
    description: string
    userId: string
    username: string
    email: string
    role: WorkspaceRole
    createdAt: string
    joinedAt: string
    addedByUsername: string
}

export interface WorkspaceMemberRemovedResponse {
    workspaceId: string
    userId: string
    username: string
    workspaceName: string
}

export interface WorkspaceMemberRoleChangedResponse {
    workspaceId: string
    userId: string
    role: WorkspaceRole
}

export interface WorkspaceOwnershipTransferredResponse {
    workspaceId: string

    previousOwnerUserId: string
    previousOwnerUsername: string

    newOwnerUserId: string
    newOwnerUsername: string
}

export type WorkspaceRole = 1 | 2 | 3