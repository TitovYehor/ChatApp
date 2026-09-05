import WorkspaceCreateForm from './WorkspaceCreateForm'
import WorkspaceItem from './WorkspaceItem'

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
}

function WorkspaceSidebar({
    workspaces,
    selectedWorkspaceId,
    isCreating,
    createError,
    updatingWorkspaceId,
    updateWorkspaceError,
    updateErrorWorkspaceId,
    onSelectWorkspace,
    onCreateWorkspace,
    onUpdateWorkspace,
}: WorkspaceSidebarProps) {
    return (
        <div>
            <h2>
                Workspaces
            </h2>

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
                <p>
                    No workspaces
                </p>
            ) : (
                <ul>
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
                                    onSelect={
                                        onSelectWorkspace
                                    }
                                    onUpdate={
                                        onUpdateWorkspace
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