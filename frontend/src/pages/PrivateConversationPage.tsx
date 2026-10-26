import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import { useAuth } from '../features/auth/useAuth'
import {
    usePrivateConversations,
} from '../features/privateConversations/usePrivateConversations'
import {
    usePrivateMessages,
    useSendPrivateMessage,
} from '../features/privateConversations/usePrivateMessages'
import PrivateMessageComposer from '../features/privateConversations/PrivateMessageComposer'
import PrivateTypingIndicator from '../features/presence/PrivateTypingIndicator'

import {
    getChatConnection,
    joinPrivateConversation,
    leavePrivateConversation,
    startChatConnection,
    subscribeToChatReconnect
} from '../services/signalR/chatConnection'

import { SignalREvents } from '../types/signalREvents'
import type { PrivateMessageResponse } from '../types/privateConversationTypes'
import { usePrivateTypingIndicator } from '../features/presence/usePrivateTypingIndicator'
import { logError } from '../utils/logger'

import './css/PrivateConversationPage.css'

interface PrivateMessagesState {
    conversationId: string | null
    messages: PrivateMessageResponse[]
}

function PrivateConversationPage() {
    const { conversationId } = useParams<{ conversationId: string }>()
    const navigate = useNavigate()
    const { user } = useAuth()

    const {
        conversations,
        isLoading: isLoadingConversations,
    } = usePrivateConversations()

    const {
        result,
        isLoading,
        error,
        refetch,
    } = usePrivateMessages(conversationId)

    const sendMessageMutation = useSendPrivateMessage(conversationId)

    const [liveState, setLiveState] = useState<PrivateMessagesState>({
        conversationId: null,
        messages: [],
    })
    const [sendError, setSendError] = useState<string | null>(null)
    const [connectionError, setConnectionError] = useState<string | null>(null)

    const {
        typingUsers,
        startTyping,
        stopTyping,
    } = usePrivateTypingIndicator(conversationId ?? null, user?.id ?? null)

    const conversation = conversations.find(
        (item) => item.id === conversationId,
    )

    const liveMessages = liveState.conversationId === conversationId
        ? liveState.messages
        : []

    const messages = useMemo(() => {
        const uniqueMessages = new Map<string, PrivateMessageResponse>()

        for (const message of result?.items ?? []) {
            uniqueMessages.set(message.id, message)
        }

        for (const message of liveMessages) {
            uniqueMessages.set(message.id, message)
        }

        return Array.from(uniqueMessages.values()).sort(
            (a, b) =>
                new Date(a.createdAt).getTime() -
                new Date(b.createdAt).getTime(),
        )
    }, [result?.items, liveMessages])

    useEffect(() => {
        if (!conversationId) {
            return
        }

        const activeConversationId = conversationId
        const connection = getChatConnection()
        let disposed = false
        let joined = false

        const handlePrivateMessageCreated = (
            message: PrivateMessageResponse,
        ) => {
            if (message.conversationId !== activeConversationId) {
                return
            }

            setLiveState((current) => {
                const existingMessages =
                    current.conversationId === activeConversationId
                        ? current.messages
                        : []

                if (existingMessages.some((item) => item.id === message.id)) {
                    return current
                }

                return {
                    conversationId: activeConversationId,
                    messages: [...existingMessages, message],
                }
            })

            void refetch()
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
                setConnectionError(null)
            } catch (error) {
                if (!disposed) {
                    logError('Failed to join private conversation group', error)
                    setConnectionError(
                        'Live updates are unavailable. Trying to reconnect may help',
                    )
                }
            }
        }

        const handleReconnected = () => {
            joined = false
            void joinGroup()
        }

        connection.on(
            SignalREvents.PrivateMessageCreated,
            handlePrivateMessageCreated,
        )
        const unsubscribeReconnect = subscribeToChatReconnect(handleReconnected)

        void startChatConnection()
            .then(joinGroup)
            .catch((error: unknown) => {
                if (!disposed) {
                    logError('Failed to connect to chat', error)
                    setConnectionError(
                        'Could not connect to live chat updates',
                    )
                }
            })

        return () => {
            disposed = true

            connection.off(
                SignalREvents.PrivateMessageCreated,
                handlePrivateMessageCreated,
            )
            unsubscribeReconnect()

            if (joined) {
                void leavePrivateConversation(activeConversationId).catch(
                    (error: unknown) => {
                        logError('Failed to leave private conversation group', error)
                    },
                )
            }
        }
    }, [conversationId, refetch])

    const handleSend = useCallback(async (content: string) => {
        setSendError(null)

        try {
            const message = await sendMessageMutation.mutateAsync(content)

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
        } catch (error) {
            logError('Failed to send private message', error)
            setSendError('Could not send your message. Please try again')
            throw error
        }
    }, [sendMessageMutation])

    if (!conversationId) {
        return (
            <main className="private-conversation">
                <p className="private-conversation__error">
                    Invalid conversation.
                </p>
                <button type="button" onClick={() => navigate('/friends')}>
                    Back to friends
                </button>
            </main>
        )
    }

    return (
        <main className="private-conversation">
            <header className="private-conversation__header">
                <button
                    type="button"
                    className="private-conversation__back"
                    onClick={() => navigate('/friends')}
                >
                    Back
                </button>

                <div className="private-conversation__avatar" aria-hidden="true">
                    {(conversation?.otherUsername ?? '?').charAt(0).toUpperCase()}
                </div>

                <div className="private-conversation__header-info">
                    <h1>
                        {conversation?.otherUsername ?? 'Private conversation'}
                    </h1>
                    <p>Private messages</p>
                </div>
            </header>

            {connectionError && (
                <p className="private-conversation__notice" role="status">
                    {connectionError}
                </p>
            )}

            <section
                className="private-conversation__messages"
                aria-label="Private messages"
            >
                {isLoading || (isLoadingConversations && !conversation) ? (
                    <p className="private-conversation__status">
                        Loading conversation…
                    </p>
                ) : error ? (
                    <div>
                        <p className="private-conversation__error">{error}</p>
                        <button type="button" onClick={() => void refetch()}>
                            Try again
                        </button>
                    </div>
                ) : messages.length === 0 ? (
                    <p className="private-conversation__status">
                        No messages yet. Start the conversation!
                    </p>
                ) : (
                    <div className="private-conversation__message-list">
                        {messages.map((message) => {
                            const isOwnMessage = message.userId === user?.id

                            return (
                                <article
                                    key={message.id}
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
                                    <p className="private-message__content">
                                        {message.content}
                                    </p>
                                </article>
                            )
                        })}
                    </div>
                )}
            </section>

            <PrivateTypingIndicator typingUsers={typingUsers} />

            <PrivateMessageComposer
                isSending={sendMessageMutation.isPending}
                sendError={sendError}
                onSend={handleSend}
                onTypingStart={startTyping}
                onTypingStop={stopTyping}
            />
        </main>
    )
}

export default PrivateConversationPage