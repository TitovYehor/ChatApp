import {
    useEffect,
} from 'react'

import {
    useMessages,
} from '../messages/useMessages'

import {
    useChannelSignalR,
} from './useChannelSignalR'

import {
    useRealtimeMessages,
} from '../messages/useRealtimeMessages'

import {
    useTypingIndicator,
} from '../presence/useTypingIndicator'

export function useChatController(
    channelId: string | null,
    currentUserId: string | null,
) {
    const {
        messages,
        isLoading: isMessagesLoading,
        error: messagesError,

        sendMessage,
        isSending,
        sendError,

        updateMessage,
        updatingMessageId,
        updateError,
        updateErrorMessageId,

        deleteMessage,
        deletingMessageId,
        deleteError,
        deleteErrorMessageId,
    } = useMessages(
        channelId,
    )

    useChannelSignalR(
        channelId,
    )

    useRealtimeMessages(
        channelId,
    )

    const {
        typingUsers,
        startTyping,
        stopTyping,
    } = useTypingIndicator(
        channelId,
        currentUserId,
    )

    useEffect(() => {
        if (!channelId) {
            return
        }

        return () => {
            void stopTyping()
        }
    }, [
        channelId,
        stopTyping,
    ])

    async function sendChatMessage(
        content: string,
    ) {
        await sendMessage(
            content,
        )
    }

    async function updateChatMessage(
        messageId: string,
        content: string,
    ) {
        await updateMessage({
            messageId,
            content,
        })
    }

    return {
        messages,
        isMessagesLoading,
        messagesError,

        sendChatMessage,
        isSending,
        sendError,

        updateChatMessage,
        updatingMessageId,
        updateError,
        updateErrorMessageId,

        deleteMessage,
        deletingMessageId,
        deleteError,
        deleteErrorMessageId,

        typingUsers,
        startTyping,
        stopTyping,
    }
}