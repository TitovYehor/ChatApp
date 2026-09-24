import {
    useState,
} from 'react'

import './css/WorkspaceItem.css'

import type {
    WorkspaceResponse,
} from '../../types/workspaceTypes'

interface WorkspaceItemProps {
    workspace: WorkspaceResponse
    isSelected: boolean
    canManageWorkspace: boolean

    isUpdating: boolean
    updateError: string | null

    isDeleting: boolean
    deleteError: string | null

    onSelect: (
        workspaceId: string,
    ) => void

    onUpdate: (
        workspaceId: string,
        name: string,
        description: string,
    ) => Promise<void>

    onDelete: (
        workspaceId: string,
    ) => Promise<void>
}

function WorkspaceItem({
    workspace,
    isSelected,
    canManageWorkspace,
    isUpdating,
    updateError,
    isDeleting,
    deleteError,
    onSelect,
    onUpdate,
    onDelete,
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

    async function handleDelete() {
        const confirmed =
            window.confirm(
                `Delete workspace "${workspace.name}"?`,
            )

        if (!confirmed) {
            return
        }

        await onDelete(
            workspace.id,
        )
    }

    return (
        <li className="workspace-item">
            {isEditing ? (
                <>
                    <input
                        className="workspace-item__input"
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
                        className="workspace-item__textarea"
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
                        className="workspace-item__action"
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
                        className="workspace-item__action"
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
                        <p className="workspace-item__error">
                            {
                                updateError
                            }
                        </p>
                    )}
                </>
            ) : (
                <>
                    <button
                        className={`workspace-item__select ${
                            isSelected
                                ? 'workspace-item__select--selected'
                                : ''
                        }`}
                        type="button"
                        onClick={() =>
                            onSelect(
                                workspace.id,
                            )
                        }
                        aria-pressed={
                            isSelected
                        }
                        disabled={
                            isDeleting
                        }
                    >
                        {
                            workspace.name
                        }
                    </button>

                    {canManageWorkspace && (
                        <>
                            <button
                                className="workspace-item__action"
                                type="button"
                                onClick={
                                    handleStartEditing
                                }
                                disabled={
                                    isUpdating ||
                                    isDeleting
                                }
                            >
                                Edit
                            </button>

                            <button
                                className="workspace-item__action workspace-item__action--danger"
                                type="button"
                                onClick={() => {
                                    void handleDelete()
                                }}
                                disabled={
                                    isUpdating ||
                                    isDeleting
                                }
                            >
                                {isDeleting
                                    ? 'Deleting...'
                                    : 'Delete'}
                            </button>

                            {deleteError && (
                                <p className="workspace-item__error">
                                    {deleteError}
                                </p>
                            )}
                        </>
                    )}
                </>
            )}
        </li>
    )
}

export default WorkspaceItem