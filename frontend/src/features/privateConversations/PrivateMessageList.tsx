import type { ReactNode } from 'react'

import PrivateMessageItem from './PrivateMessageItem'

import type { PrivateMessageResponse } from '../../types/privateConversationTypes'

interface PrivateMessageListProps {
    messages: PrivateMessageResponse[]
    currentUserId: string | undefined
    isLoading: boolean
    error: string | null
    isSaving: boolean
    isDeleting: boolean
    onRetry: () => void
    onSave: (message: PrivateMessageResponse, content: string) => Promise<void>
    onDelete: (message: PrivateMessageResponse) => Promise<void>
}

export default function PrivateMessageList({
    messages,
    currentUserId,
    isLoading,
    error,
    isSaving,
    isDeleting,
    onRetry,
    onSave,
    onDelete,
}: PrivateMessageListProps) {
    let content: ReactNode

    if (isLoading) {
        content = (
            <p className="private-conversation__status">
                Loading conversation…
            </p>
        )
    } else if (error) {
        content = (
            <div>
                <p className="private-conversation__error">{error}</p>
                <button type="button" onClick={onRetry}>
                    Try again
                </button>
            </div>
        )
    } else if (messages.length === 0) {
        content = (
            <p className="private-conversation__status">
                No messages yet. Start the conversation!
            </p>
        )
    } else {
        content = (
            <div className="private-conversation__message-list">
                {messages.map((message) => (
                    <PrivateMessageItem
                        key={message.id}
                        message={message}
                        isOwnMessage={message.userId === currentUserId}
                        isSaving={isSaving}
                        isDeleting={isDeleting}
                        onSave={onSave}
                        onDelete={onDelete}
                    />
                ))}
            </div>
        )
    }

    return (
        <section
            className="private-conversation__messages"
            aria-label="Private messages"
        >
            {content}
        </section>
    )
}