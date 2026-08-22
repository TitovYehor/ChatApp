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

export type WorkspaceRole =
    | 'Owner'
    | 'Admin'
    | 'Member'