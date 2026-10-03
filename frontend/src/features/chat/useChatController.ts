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
    searchQuery: string,
) {
    const {
        messages,

        pageNumber,
        pageSize,
        totalCount,
        totalPages,
        hasPreviousPage,
        hasNextPage,
        goToPreviousPage,
        goToNextPage,
        isFetching: isMessagesFetching,

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
        searchQuery,
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

        pageNumber,
        pageSize,
        totalCount,
        totalPages,
        hasPreviousPage,
        hasNextPage,
        goToPreviousPage,
        goToNextPage,
        isMessagesFetching,

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