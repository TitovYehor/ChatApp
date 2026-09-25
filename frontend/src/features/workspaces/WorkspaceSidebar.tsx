import WorkspaceCreateForm from './WorkspaceCreateForm'
import WorkspaceItem from './WorkspaceItem'

import './css/WorkspaceSidebar.css'

import type {
    WorkspaceResponse,
} from '../../types/workspaceTypes'

interface WorkspaceSidebarProps {
    workspaces: WorkspaceResponse[]
    selectedWorkspaceId: string | null

    isCreating: boolean
    createError: string | null

    updatingWorkspaceId: string | null
    updateWorkspaceError: string | null
    updateErrorWorkspaceId: string | null

    deletingWorkspaceId: string | null
    deleteWorkspaceError: string | null
    deleteErrorWorkspaceId: string | null

    onSelectWorkspace: (
        workspaceId: string,
    ) => void

    onCreateWorkspace: (
        name: string,
        description: string,
    ) => Promise<void>

    onUpdateWorkspace: (
        workspaceId: string,
        name: string,
        description: string,
    ) => Promise<void>

    onDeleteWorkspace: (
        workspaceId: string,
    ) => Promise<void>
}

function WorkspaceSidebar({
    workspaces,
    selectedWorkspaceId,
    isCreating,
    createError,
    updatingWorkspaceId,
    updateWorkspaceError,
    updateErrorWorkspaceId,
    deletingWorkspaceId,
    deleteWorkspaceError,
    deleteErrorWorkspaceId,
    onSelectWorkspace,
    onCreateWorkspace,
    onUpdateWorkspace,
    onDeleteWorkspace
}: WorkspaceSidebarProps) {
    return (
        <div className="workspace-sidebar">
            <div className="workspace-sidebar__header">
                <h2>
                    Workspaces
                </h2>
            </div>

            <WorkspaceCreateForm
                isCreating={
                    isCreating
                }
                createError={
                    createError
                }
                onCreate={
                    onCreateWorkspace
                }
            />

            {workspaces.length ===
                0 ? (
                <p className="workspace-sidebar__empty">
                    No workspaces
                </p>
            ) : (
                <ul className="workspace-sidebar__list">
                    {workspaces.map(
                        (
                            workspace,
                        ) => {
                            const isSelected =
                                workspace.id ===
                                selectedWorkspaceId

                            const canManageWorkspace =
                                workspace.currentUserRole === 1

                            return (
                                <WorkspaceItem
                                    key={
                                        workspace.id
                                    }
                                    workspace={
                                        workspace
                                    }
                                    isSelected={
                                        isSelected
                                    }
                                    canManageWorkspace={
                                        canManageWorkspace
                                    }
                                    isUpdating={
                                        updatingWorkspaceId ===
                                        workspace.id
                                    }
                                    updateError={
                                        updateErrorWorkspaceId ===
                                            workspace.id
                                            ? updateWorkspaceError
                                            : null
                                    }
                                    isDeleting={
                                        deletingWorkspaceId ===
                                        workspace.id
                                    }
                                    deleteError={
                                        deleteErrorWorkspaceId ===
                                            workspace.id
                                            ? deleteWorkspaceError
                                            : null
                                    }
                                    onSelect={
                                        onSelectWorkspace
                                    }
                                    onUpdate={
                                        onUpdateWorkspace
                                    }
                                    onDelete={
                                        onDeleteWorkspace
                                    }
                                />
                            )
                        },
                    )}
                </ul>
            )}
        </div>
    )
}

export default WorkspaceSidebar