import {
    useState,
} from 'react'

import './css/MessageItem.css'

import type {
    MessageResponse,
} from '../../types/messageTypes'

interface MessageItemProps {
    message: MessageResponse
    currentUserId: string | null
    canManageMessages: boolean

    onUpdate: (
        messageId: string,
        content: string,
    ) => Promise<void>

    onDelete: (
        messageId: string,
    ) => Promise<void>

    isUpdating: boolean
    isDeleting: boolean

    updateError: string | null
    deleteError: string | null
}

function MessageItem({
    message,
    currentUserId,
    canManageMessages,
    onUpdate,
    onDelete,
    isUpdating,
    isDeleting,
    updateError,
    deleteError,
}: MessageItemProps) {
    const [
        isEditing,
        setIsEditing,
    ] = useState(false)

    const [
        editingContent,
        setEditingContent,
    ] = useState(message.content)

    const isOwnMessage = currentUserId === message.userId

    const canEditMessage = isOwnMessage

    const canDeleteMessage =
        isOwnMessage ||
        canManageMessages

    const handleStartEditing = () => {
        setEditingContent(
            message.content,
        )

        setIsEditing(true)
    }

    const handleCancelEditing = () => {
        setEditingContent(
            message.content,
        )

        setIsEditing(false)
    }

    const handleSaveEditing = async () => {
        const content = editingContent.trim()

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
        await onDelete(
            message.id,
        )
    }

    return (
        <li className="message-item">
            {isEditing ? (
                <div className="message-item__editing">
                    <strong className="message-item__username">
                        {message.username}
                    </strong>

                    <input
                        className="message-item__edit-input"
                        type="text"
                        value={editingContent}
                        onChange={(event) => {
                            setEditingContent(
                                event.target.value,
                            )
                        }}
                        disabled={isUpdating}
                    />

                    <div className="message-item__actions">
                        <button
                            type="button"
                            className="message-item__action message-item__action--primary"
                            onClick={() => {
                                void handleSaveEditing()
                            }}
                            disabled={
                                isUpdating ||
                                editingContent
                                    .trim()
                                    .length === 0
                            }
                        >
                            {isUpdating
                                ? 'Saving...'
                                : 'Save'}
                        </button>

                        <button
                            type="button"
                            className="message-item__action"
                            onClick={
                                handleCancelEditing
                            }
                            disabled={isUpdating}
                        >
                            Cancel
                        </button>
                    </div>

                    {updateError && (
                        <p className="message-item__error">
                            {updateError}
                        </p>
                    )}
                </div>
            ) : (
                <>
                    <div className="message-item__header">
                        <strong className="message-item__username">
                            {message.username}
                        </strong>

                        {message.updatedAt && (
                            <span className="message-item__edited">
                                edited
                            </span>
                        )}
                    </div>

                    <p className="message-item__content">
                        {message.content}
                    </p>

                    {(canEditMessage || canDeleteMessage) && (
                        <div className="message-item__actions">
                            {canEditMessage && (
                                <button
                                    type="button"
                                    className="message-item__action"
                                    onClick={
                                        handleStartEditing
                                    }
                                    disabled={
                                        isDeleting
                                    }
                                >
                                    Edit
                                </button>
                            )}

                            {canDeleteMessage && (
                                <button
                                    type="button"
                                    className="message-item__action message-item__action--danger"
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
                            )}
                        </div>
                    )}

                    {deleteError && (
                        <p className="message-item__error">
                            {deleteError}
                        </p>
                    )}
                </>
            )}
        </li>
    )
}

export default MessageItem