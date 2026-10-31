import { useState } from 'react'
import type { FormEvent } from 'react'

import type { PrivateMessageResponse } from '../../types/privateConversationTypes'

interface PrivateMessageItemProps {
    message: PrivateMessageResponse
    isOwnMessage: boolean
    isSaving: boolean
    isDeleting: boolean
    onSave: (message: PrivateMessageResponse, content: string) => Promise<void>
    onDelete: (message: PrivateMessageResponse) => Promise<void>
}

export default function PrivateMessageItem({
    message,
    isOwnMessage,
    isSaving,
    isDeleting,
    onSave,
    onDelete,
}: PrivateMessageItemProps) {
    const [isEditing, setIsEditing] = useState(false)
    const [editContent, setEditContent] = useState(message.content)
    const [actionError, setActionError] = useState<string | null>(null)

    function handleStartEdit() {
        setEditContent(message.content)
        setActionError(null)
        setIsEditing(true)
    }

    function handleCancelEdit() {
        setIsEditing(false)
        setEditContent(message.content)
        setActionError(null)
    }

    async function handleSave(event: FormEvent<HTMLFormElement>) {
        event.preventDefault()

        const content = editContent.trim()

        if (!content) {
            setActionError('Message content cannot be empty')
            return
        }

        if (content === message.content) {
            handleCancelEdit()
            return
        }

        setActionError(null)

        try {
            await onSave(message, content)
            setIsEditing(false)
        } catch {
            setActionError('Could not edit the message. Please try again')
        }
    }

    async function handleDelete() {
        const confirmed = window.confirm(
            'Are you sure you want to delete this message? This cannot be undone',
        )

        if (!confirmed) {
            return
        }

        setActionError(null)

        try {
            await onDelete(message)
        } catch {
            setActionError('Could not delete the message. Please try again')
        }
    }

    return (
        <article
            className={
                isOwnMessage
                    ? 'private-message private-message--own'
                    : 'private-message'
            }
        >
            <div className="private-message__meta">
                <span>{isOwnMessage ? 'You' : message.username}</span>
                <time dateTime={message.createdAt}>
                    {new Date(message.createdAt).toLocaleString()}
                </time>
            </div>

            {isEditing ? (
                <form
                    className="private-message__edit-form"
                    onSubmit={(event) => void handleSave(event)}
                >
                    <textarea
                        className="private-message__edit-input"
                        value={editContent}
                        onChange={(event) => setEditContent(event.target.value)}
                        rows={3}
                        maxLength={4000}
                        autoFocus
                        aria-label="Edit message"
                        disabled={isSaving}
                    />

                    <div className="private-message__actions">
                        <button type="submit" disabled={isSaving}>
                            {isSaving ? 'Saving…' : 'Save'}
                        </button>

                        <button
                            type="button"
                            onClick={handleCancelEdit}
                            disabled={isSaving}
                        >
                            Cancel
                        </button>
                    </div>
                </form>
            ) : (
                <>
                    <p className="private-message__content">
                        {message.content}
                    </p>

                    {message.updatedAt && (
                        <span className="private-message__edited">
                            Edited
                        </span>
                    )}

                    {isOwnMessage && (
                        <div className="private-message__actions">
                            <button
                                type="button"
                                onClick={handleStartEdit}
                                disabled={isDeleting}
                            >
                                Edit
                            </button>

                            <button
                                type="button"
                                onClick={() => void handleDelete()}
                                disabled={isDeleting}
                            >
                                {isDeleting ? 'Deleting…' : 'Delete'}
                            </button>
                        </div>
                    )}
                </>
            )}

            {actionError && (
                <p className="private-conversation__error" role="alert">
                    {actionError}
                </p>
            )}
        </article>
    )
}