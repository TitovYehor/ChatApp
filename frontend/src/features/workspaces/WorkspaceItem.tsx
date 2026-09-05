import {
    useState,
} from 'react'

import type {
    WorkspaceResponse,
} from '../../types/workspaceTypes'

interface WorkspaceItemProps {
    workspace: WorkspaceResponse
    isSelected: boolean
    canManageWorkspace: boolean

    isUpdating: boolean
    updateError: string | null

    onSelect: (
        workspaceId: string,
    ) => void

    onUpdate: (
        workspaceId: string,
        name: string,
        description: string,
    ) => Promise<void>
}

function WorkspaceItem({
    workspace,
    isSelected,
    canManageWorkspace,
    isUpdating,
    updateError,
    onSelect,
    onUpdate,
}: WorkspaceItemProps) {
    const [
        isEditing,
        setIsEditing,
    ] = useState(false)

    const [
        editingName,
        setEditingName,
    ] = useState(
        workspace.name,
    )

    const [
        editingDescription,
        setEditingDescription,
    ] = useState(
        workspace.description,
    )

    function handleStartEditing() {
        setEditingName(
            workspace.name,
        )

        setEditingDescription(
            workspace.description,
        )

        setIsEditing(true)
    }

    function handleCancelEditing() {
        setEditingName(
            workspace.name,
        )

        setEditingDescription(
            workspace.description,
        )

        setIsEditing(false)
    }

    async function handleSaveEditing() {
        const name =
            editingName.trim()

        const description =
            editingDescription.trim()

        if (!name) {
            return
        }

        await onUpdate(
            workspace.id,
            name,
            description,
        )

        setIsEditing(false)
    }

    return (
        <li>
            {isEditing ? (
                <>
                    <input
                        type="text"
                        value={
                            editingName
                        }
                        onChange={(
                            event,
                        ) =>
                            setEditingName(
                                event.target
                                    .value,
                            )
                        }
                        maxLength={100}
                        disabled={
                            isUpdating
                        }
                    />

                    <textarea
                        value={
                            editingDescription
                        }
                        onChange={(
                            event,
                        ) =>
                            setEditingDescription(
                                event.target
                                    .value,
                            )
                        }
                        disabled={
                            isUpdating
                        }
                    />

                    <button
                        type="button"
                        onClick={() => {
                            void handleSaveEditing()
                        }}
                        disabled={
                            isUpdating
                        }
                    >
                        {isUpdating
                            ? 'Saving...'
                            : 'Save'}
                    </button>

                    <button
                        type="button"
                        onClick={
                            handleCancelEditing
                        }
                        disabled={
                            isUpdating
                        }
                    >
                        Cancel
                    </button>

                    {updateError && (
                        <p>
                            {
                                updateError
                            }
                        </p>
                    )}
                </>
            ) : (
                <>
                    <button
                        type="button"
                        onClick={() =>
                            onSelect(
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

                    {canManageWorkspace && (
                        <button
                            type="button"
                            onClick={
                                handleStartEditing
                            }
                            disabled={
                                isUpdating
                            }
                        >
                            Edit
                        </button>
                    )}
                </>
            )}
        </li>
    )
}

export default WorkspaceItem