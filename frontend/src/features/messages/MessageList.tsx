import MessageItem from './MessageItem'

import type {
    MessageResponse,
} from '../../types/messageTypes'

interface MessageListProps {
    messages: MessageResponse[]
    currentUserId: string | null

    updatingMessageId: string | null
    deletingMessageId: string | null

    onUpdate: (
        messageId: string,
        content: string,
    ) => Promise<void>

    onDelete: (
        messageId: string,
    ) => Promise<void>
}

function MessageList({
    messages,
    currentUserId,
    updatingMessageId,
    deletingMessageId,
    onUpdate,
    onDelete,
}: MessageListProps) {
    return (
        <ul>
            {messages.map(
                (message) => (
                    <MessageItem
                        key={
                            message.id
                        }
                        message={
                            message
                        }
                        currentUserId={
                            currentUserId
                        }
                        isUpdating={
                            updatingMessageId ===
                            message.id
                        }
                        isDeleting={
                            deletingMessageId ===
                            message.id
                        }
                        onUpdate={
                            onUpdate
                        }
                        onDelete={
                            onDelete
                        }
                    />
                ),
            )}
        </ul>
    )
}

export default MessageList