import {
    useWorkspaces,
} from './useWorkspaces'

import {
    useWorkspaceMembers,
} from './useWorkspaceMembers'

import type {
    WorkspaceRole,
} from '../../types/workspaceTypes'

import {
    useRealtimeWorkspaces,
} from './useRealtimeWorkspaces'

export function useWorkspaceController(
    selectedWorkspaceId: string | null,
) {
    useRealtimeWorkspaces()

    const {
        workspaces,
        isLoading: isLoadingWorkspaces,
        error: workspacesError,

        createWorkspace,
        isCreating: isCreatingWorkspace,
        createError: createWorkspaceError,

        updateWorkspace,
        updatingWorkspaceId,
        updateWorkspaceError,
        updateErrorWorkspaceId,

        deleteWorkspace,
        deletingWorkspaceId,
        deleteWorkspaceError,
        deleteErrorWorkspaceId,

        leaveWorkspace,
        isLeaving,
        leavingWorkspaceId,
        leaveWorkspaceError,
    } = useWorkspaces()

    const selectedWorkspace =
        workspaces.find(
            (workspace) =>
                workspace.id ===
                selectedWorkspaceId,
        )

    const {
        members,
        isLoading: isLoadingMembers,
        error: membersError,

        addMember,
        isAdding: isAddingMember,
        addError: addMemberError,

        removeMember,
        isRemoving: isRemovingMember,
        removingMember,
        removeError: removeMemberError,

        changeMemberRole,
        isChangingMemberRole,
        changingMemberRole,
        changeMemberRoleError,

        transferOwnership,
        isTransferringOwnership,
        transferringOwnership,
        transferOwnershipError,
    } = useWorkspaceMembers(
        selectedWorkspaceId,
    )

    const canManageChannels =
        selectedWorkspace?.currentUserRole === 1 ||
        selectedWorkspace?.currentUserRole === 2

    const canManageMembers =
        selectedWorkspace?.currentUserRole === 1

    const canManageMessages =
        selectedWorkspace?.currentUserRole === 1 ||
        selectedWorkspace?.currentUserRole === 2

    async function createWorkspaceAndReturn(
        name: string,
        description: string,
    ) {
        return createWorkspace({
            name,
            description,
        })
    }

    async function updateWorkspaceDetails(
        workspaceId: string,
        name: string,
        description: string,
    ) {
        await updateWorkspace({
            workspaceId,
            name,
            description,
        })
    }

    async function deleteWorkspaceById(
        workspaceId: string,
    ) {
        await deleteWorkspace(
            workspaceId,
        )
    }

    async function leaveSelectedWorkspace() {
        if (!selectedWorkspaceId) {
            return
        }

        await leaveWorkspace(
            selectedWorkspaceId,
        )
    }

    async function changeWorkspaceMemberRole(
        usernameOrEmail: string,
        role: WorkspaceRole,
    ) {
        await changeMemberRole({
            usernameOrEmail,
            role,
        })
    }

    return {
        workspaces,
        isLoadingWorkspaces,
        workspacesError,

        selectedWorkspace,

        canManageChannels,
        canManageMembers,
        canManageMessages,

        createWorkspaceAndReturn,
        isCreatingWorkspace,
        createWorkspaceError,

        updateWorkspaceDetails,
        updatingWorkspaceId,
        updateWorkspaceError,
        updateErrorWorkspaceId,

        deleteWorkspaceById,
        deletingWorkspaceId,
        deleteWorkspaceError,
        deleteErrorWorkspaceId,

        leaveSelectedWorkspace,
        isLeaving,
        leavingWorkspaceId,
        leaveWorkspaceError,

        members,
        isLoadingMembers,
        membersError,

        addMember,
        isAddingMember,
        addMemberError,

        removeMember,
        isRemovingMember,
        removingMember,
        removeMemberError,

        changeWorkspaceMemberRole,
        isChangingMemberRole,
        changingMemberRole,
        changeMemberRoleError,

        transferOwnership,
        isTransferringOwnership,
        transferringOwnership,
        transferOwnershipError,
    }
}