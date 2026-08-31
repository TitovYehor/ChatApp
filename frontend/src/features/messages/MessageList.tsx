import MessageItem from './MessageItem'

import type {
    MessageResponse,
} from '../../types/messageTypes'

interface MessageListProps {
    messages: MessageResponse[]
    currentUserId: string | null

    isUpdating: boolean
    isDeleting: boolean

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
    isUpdating,
    isDeleting,
    onUpdate,
    onDelete,
}: MessageListProps) {
    return (
        <ul>
            {messages.map((message) => (
                <MessageItem
                    key={message.id}
                    message={message}
                    currentUserId={
                        currentUserId
                    }
                    isUpdating={
                        isUpdating
                    }
                    isDeleting={
                        isDeleting
                    }
                    onUpdate={
                        onUpdate
                    }
                    onDelete={
                        onDelete
                    }
                />
            ))}
        </ul>
    )
}

export default MessageList