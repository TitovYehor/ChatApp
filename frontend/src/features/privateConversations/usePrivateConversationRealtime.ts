import { useCallback, useEffect, useState } from 'react'

import {
    getChatConnection,
    joinPrivateConversation,
    leavePrivateConversation,
    startChatConnection,
    subscribeToChatReconnect,
} from '../../services/signalR/chatConnection'

import { SignalREvents } from '../../types/signalREvents'

import type {
    PrivateMessageDeletedResponse,
    PrivateMessageResponse,
} from '../../types/privateConversationTypes'

import { logError } from '../../utils/logger'

interface LiveMessagesState {
    conversationId: string | null
    messages: PrivateMessageResponse[]
}

interface UsePrivateConversationRealtimeOptions {
    conversationId: string | undefined
    onMessagesChanged: () => void
}

export function usePrivateConversationRealtime({
    conversationId,
    onMessagesChanged,
}: UsePrivateConversationRealtimeOptions) {
    const [liveState, setLiveState] = useState<LiveMessagesState>({
        conversationId: null,
        messages: [],
    })

    const [deletedMessageIds, setDeletedMessageIds] = useState<Set<string>>(
        () => new Set(),
    )

    const [connectionErrorState, setConnectionErrorState] = useState<{
        conversationId: string
        message: string
    } | null>(null)

    const connectionError =
        connectionErrorState?.conversationId === conversationId
            ? connectionErrorState?.message
            : null

    const addLiveMessage = useCallback((message: PrivateMessageResponse) => {
        setLiveState((current) => {
            const existingMessages =
                current.conversationId === message.conversationId
                    ? current.messages
                    : []

            if (existingMessages.some((item) => item.id === message.id)) {
                return current
            }

            return {
                conversationId: message.conversationId,
                messages: [...existingMessages, message],
            }
        })

        setDeletedMessageIds((current) => {
            if (!current.has(message.id)) {
                return current
            }

            const next = new Set(current)
            next.delete(message.id)
            return next
        })
    }, [])

    const upsertLiveMessage = useCallback((message: PrivateMessageResponse) => {
        setLiveState((current) => {
            const existingMessages =
                current.conversationId === message.conversationId
                    ? current.messages
                    : []

            const exists = existingMessages.some(
                (item) => item.id === message.id,
            )

            return {
                conversationId: message.conversationId,
                messages: exists
                    ? existingMessages.map((item) =>
                        item.id === message.id ? message : item,
                    )
                    : [...existingMessages, message],
            }
        })

        setDeletedMessageIds((current) => {
            if (!current.has(message.id)) {
                return current
            }

            const next = new Set(current)
            next.delete(message.id)
            return next
        })
    }, [])

    const removeLiveMessage = useCallback((
        messageId: string,
        targetConversationId: string,
    ) => {
        setDeletedMessageIds((current) => {
            if (current.has(messageId)) {
                return current
            }

            const next = new Set(current)
            next.add(messageId)
            return next
        })

        setLiveState((current) => {
            if (current.conversationId !== targetConversationId) {
                return current
            }

            return {
                ...current,
                messages: current.messages.filter(
                    (message) => message.id !== messageId,
                ),
            }
        })
    }, [])

    useEffect(() => {
        if (!conversationId) {
            return
        }

        const activeConversationId = conversationId
        const connection = getChatConnection()

        let disposed = false
        let joined = false

        const handleMessageCreated = (message: PrivateMessageResponse) => {
            if (message.conversationId !== activeConversationId) {
                return
            }

            addLiveMessage(message)
            onMessagesChanged()
        }

        const handleMessageUpdated = (message: PrivateMessageResponse) => {
            if (message.conversationId !== activeConversationId) {
                return
            }

            upsertLiveMessage(message)
            onMessagesChanged()
        }

        const handleMessageDeleted = (
            response: PrivateMessageDeletedResponse,
        ) => {
            if (response.conversationId !== activeConversationId) {
                return
            }

            removeLiveMessage(response.messageId, activeConversationId)
            onMessagesChanged()
        }

        const joinGroup = async () => {
            if (disposed) {
                return
            }

            try {
                await joinPrivateConversation(activeConversationId)

                if (disposed) {
                    await leavePrivateConversation(activeConversationId)
                    return
                }

                joined = true
            } catch (error) {
                if (!disposed) {
                    logError(
                        'Failed to join private conversation group',
                        error,
                    )
                    setConnectionErrorState({
                        conversationId: activeConversationId,
                        message:
                            'Live updates are unavailable. Trying to reconnect may help',
                    })
                }
            }
        }

        const handleReconnected = () => {
            joined = false
            void joinGroup()
        }

        connection.on(
            SignalREvents.PrivateMessageCreated,
            handleMessageCreated,
        )
        connection.on(
            SignalREvents.PrivateMessageUpdated,
            handleMessageUpdated,
        )
        connection.on(
            SignalREvents.PrivateMessageDeleted,
            handleMessageDeleted,
        )

        const unsubscribeReconnect =
            subscribeToChatReconnect(handleReconnected)

        void startChatConnection()
            .then(joinGroup)
            .catch((error: unknown) => {
                if (!disposed) {
                    logError('Failed to connect to chat', error)
                    setConnectionErrorState({
                        conversationId: activeConversationId,
                        message: 'Could not connect to live chat updates',
                    })
                }
            })

        return () => {
            disposed = true

            connection.off(
                SignalREvents.PrivateMessageCreated,
                handleMessageCreated,
            )
            connection.off(
                SignalREvents.PrivateMessageUpdated,
                handleMessageUpdated,
            )
            connection.off(
                SignalREvents.PrivateMessageDeleted,
                handleMessageDeleted,
            )

            unsubscribeReconnect()

            if (joined) {
                void leavePrivateConversation(activeConversationId).catch(
                    (error: unknown) => {
                        logError(
                            'Failed to leave private conversation group',
                            error,
                        )
                    },
                )
            }
        }
    }, [
        conversationId,
        onMessagesChanged,
        addLiveMessage,
        upsertLiveMessage,
        removeLiveMessage,
    ])

    const liveMessages =
        liveState.conversationId === conversationId
            ? liveState.messages
            : []

    return {
        liveMessages,
        deletedMessageIds,
        connectionError,
        addLiveMessage,
        upsertLiveMessage,
        removeLiveMessage,
    }
}