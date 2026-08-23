import type { WorkspaceResponse } from '../../types/workspaceTypes'

interface WorkspaceSidebarProps {
    workspaces: WorkspaceResponse[]
    selectedWorkspaceId: string | null
    onSelectWorkspace: (
        workspaceId: string,
    ) => void
}

function WorkspaceSidebar({
    workspaces,
    selectedWorkspaceId,
    onSelectWorkspace,
}: WorkspaceSidebarProps) {
    return (
        <div>
            <h2>Workspaces</h2>

            {workspaces.length === 0 ? (
                <p>No workspaces</p>
            ) : (
                <ul>
                    {workspaces.map((workspace) => {
                        const isSelected =
                            workspace.id ===
                            selectedWorkspaceId

                        return (
                            <li key={workspace.id}>
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
                                    {workspace.name}
                                </button>
                            </li>
                        )
                    })}
                </ul>
            )}
        </div>
    )
}

export default WorkspaceSidebar