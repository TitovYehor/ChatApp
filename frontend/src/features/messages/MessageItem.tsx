import {
    useState,
} from 'react'

import type {
    MessageResponse,
} from '../../types/messageTypes'

interface MessageItemProps {
    message: MessageResponse
    currentUserId: string | null
    onUpdate: (
        messageId: string,
        content: string,
    ) => Promise<void>
    onDelete: (
        messageId: string,
    ) => Promise<void>
    isUpdating: boolean
    isDeleting: boolean
}

function MessageItem({
    message,
    currentUserId,
    onUpdate,
    onDelete,
    isUpdating,
    isDeleting,
}: MessageItemProps) {
    const [
        isEditing,
        setIsEditing,
    ] = useState(false)

    const [
        editingContent,
        setEditingContent,
    ] = useState(message.content)

    const isOwnMessage =
        currentUserId === message.userId

    const handleStartEditing = () => {
        setEditingContent(message.content)
        setIsEditing(true)
    }

    const handleCancelEditing = () => {
        setEditingContent(message.content)
        setIsEditing(false)
    }

    const handleSaveEditing = async () => {
        const content =
            editingContent.trim()

        if (!content) {
            return
        }

        await onUpdate(
            message.id,
            content,
        )

        setIsEditing(false)
    }

    const handleDelete = async () => {
        await onDelete(message.id)
    }

    return (
        <li>
            {isEditing ? (
                <>
                    <strong>
                        {message.username}
                    </strong>

                    <input
                        type="text"
                        value={editingContent}
                        onChange={(event) => {
                            setEditingContent(
                                event.target.value,
                            )
                        }}
                        disabled={isUpdating}
                    />

                    <button
                        type="button"
                        onClick={() => {
                            void handleSaveEditing()
                        }}
                        disabled={
                            isUpdating ||
                            editingContent.trim()
                                .length === 0
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
                        disabled={isUpdating}
                    >
                        Cancel
                    </button>
                </>
            ) : (
                <>
                    <strong>
                        {message.username}
                    </strong>
                    : {message.content}

                    {message.updatedAt && (
                        <span>
                            {' '}
                            (edited)
                        </span>
                    )}

                    {isOwnMessage && (
                        <>
                            <button
                                type="button"
                                onClick={
                                    handleStartEditing
                                }
                                disabled={
                                    isDeleting
                                }
                            >
                                Edit
                            </button>

                            <button
                                type="button"
                                onClick={() => {
                                    void handleDelete()
                                }}
                                disabled={
                                    isDeleting
                                }
                            >
                                {isDeleting
                                    ? 'Deleting...'
                                    : 'Delete'}
                            </button>
                        </>
                    )}
                </>
            )}
        </li>
    )
}

export default MessageItem