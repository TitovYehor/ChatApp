import WorkspaceCreateForm from './WorkspaceCreateForm'

import type {
    WorkspaceResponse,
} from '../../types/workspaceTypes'

interface WorkspaceSidebarProps {
    workspaces: WorkspaceResponse[]
    selectedWorkspaceId: string | null

    isCreating: boolean
    createError: string | null

    onSelectWorkspace: (
        workspaceId: string,
    ) => void

    onCreateWorkspace: (
        name: string,
        description: string,
    ) => Promise<void>
}

function WorkspaceSidebar({
    workspaces,
    selectedWorkspaceId,
    isCreating,
    createError,
    onSelectWorkspace,
    onCreateWorkspace,
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

                            return (
                                <li
                                    key={
                                        workspace.id
                                    }
                                >
                                    <button
                                        type="button"
                                        onClick={() =>
                                            onSelectWorkspace(
                                                workspace.id,
                                            )
                                        }
                                        aria-pressed={
                                            isSelected
                                        }
                                    >
                                        {
                                            workspace.name
                                        }
                                    </button>
                                </li>
                            )
                        },
                    )}
                </ul>
            )}
        </div>
    )
}

export default WorkspaceSidebar