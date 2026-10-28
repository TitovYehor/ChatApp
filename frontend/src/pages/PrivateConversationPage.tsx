import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import { useAuth } from '../features/auth/useAuth'
import {
    usePrivateConversations,
} from '../features/privateConversations/usePrivateConversations'
import {
    usePrivateMessages,
    useSendPrivateMessage,
    useUpdatePrivateMessage,
    useDeletePrivateMessage,
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
import type {
    PrivateMessageResponse,
    PrivateMessageDeletedResponse
} from '../types/privateConversationTypes'
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

    const updateMessageMutation = useUpdatePrivateMessage(conversationId)

    const deleteMessageMutation = useDeletePrivateMessage(conversationId)

    const [editingMessageId, setEditingMessageId] = useState<string | null>(null)

    const [editContent, setEditContent] = useState('')
    const [messageActionError, setMessageActionError] = useState<string | null>(null)

    const [liveState, setLiveState] = useState<PrivateMessagesState>({
        conversationId: null,
        messages: [],
    })
    const [deletedMessageIds, setDeletedMessageIds] = useState<Set<string>>(() => new Set())
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

    const messages = useMemo(() => {
        const uniqueMessages = new Map<string, PrivateMessageResponse>()

        for (const message of result?.items ?? []) {
            uniqueMessages.set(message.id, message)
        }

        const liveMessages =
            liveState.conversationId === conversationId
                ? liveState.messages
                : []

        for (const message of liveMessages) {
            uniqueMessages.set(message.id, message)
        }

        return Array.from(uniqueMessages.values())
            .filter((message) => !deletedMessageIds.has(message.id))
            .sort(
                (a, b) =>
                    new Date(a.createdAt).getTime() -
                    new Date(b.createdAt).getTime(),
            )
    }, [
        result?.items,
        liveState.conversationId,
        liveState.messages,
        conversationId,
        deletedMessageIds,
    ])

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

        const handlePrivateMessageUpdated = (
            message: PrivateMessageResponse,
        ) => {
            if (message.conversationId !== activeConversationId) {
                return
            }

            setDeletedMessageIds((current) => {
                const next = new Set(current)
                next.delete(message.id)
                return next
            })

            setLiveState((current) => {
                const existingMessages =
                    current.conversationId === activeConversationId
                        ? current.messages
                        : []

                const messageExists = existingMessages.some(
                    (item) => item.id === message.id,
                )

                return {
                    conversationId: activeConversationId,
                    messages: messageExists
                        ? existingMessages.map((item) =>
                            item.id === message.id ? message : item,
                        )
                        : [...existingMessages, message],
                }
            })

            void refetch()
        }

        const handlePrivateMessageDeleted = (
            response: PrivateMessageDeletedResponse,
        ) => {
            if (response.conversationId !== activeConversationId) {
                return
            }

            setDeletedMessageIds((current) => {
                const next = new Set(current)
                next.add(response.messageId)
                return next
            })

            setLiveState((current) => {
                if (current.conversationId !== activeConversationId) {
                    return current
                }

                return {
                    ...current,
                    messages: current.messages.filter(
                        (message) => message.id !== response.messageId,
                    ),
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

        connection.on(
            SignalREvents.PrivateMessageUpdated,
            handlePrivateMessageUpdated,
        )

        connection.on(
            SignalREvents.PrivateMessageDeleted,
            handlePrivateMessageDeleted,
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

            connection.off(
                SignalREvents.PrivateMessageUpdated,
                handlePrivateMessageUpdated,
            )

            connection.off(
                SignalREvents.PrivateMessageDeleted,
                handlePrivateMessageDeleted,
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

    const handleStartEdit = (message: PrivateMessageResponse) => {
        setMessageActionError(null)
        setEditingMessageId(message.id)
        setEditContent(message.content)
    }

    const handleCancelEdit = () => {
        setEditingMessageId(null)
        setEditContent('')
        setMessageActionError(null)
    }

    const handleSaveEdit = async (message: PrivateMessageResponse) => {
        const content = editContent.trim()

        if (!content) {
            setMessageActionError('Message content cannot be empty')
            return
        }

        if (content === message.content) {
            handleCancelEdit()
            return
        }

        setMessageActionError(null)

        try {
            const updatedMessage = await updateMessageMutation.mutateAsync({
                messageId: message.id,
                content,
            })

            setLiveState((current) => {
                const existingMessages =
                    current.conversationId === conversationId
                        ? current.messages
                        : []

                const exists = existingMessages.some(
                    (item) => item.id === updatedMessage.id,
                )

                return {
                    conversationId: conversationId!,
                    messages: exists
                        ? existingMessages.map((item) =>
                            item.id === updatedMessage.id
                                ? updatedMessage
                                : item,
                        )
                        : [...existingMessages, updatedMessage],
                }
            })

            setEditingMessageId(null)
            setEditContent('')
        } catch (error) {
            logError('Failed to edit private message', error)
            setMessageActionError('Could not edit the message. Please try again')
        }
    }

    const handleDeleteMessage = async (message: PrivateMessageResponse) => {
        const confirmed = window.confirm(
            'Are you sure you want to delete this message? This cannot be undone',
        )

        if (!confirmed) {
            return
        }

        setMessageActionError(null)

        try {
            await deleteMessageMutation.mutateAsync(message.id)

            setDeletedMessageIds((current) => {
                const next = new Set(current)
                next.add(message.id)
                return next
            })

            setLiveState((current) => {
                if (current.conversationId !== conversationId) {
                    return current
                }

                return {
                    ...current,
                    messages: current.messages.filter(
                        (item) => item.id !== message.id,
                    ),
                }
            })

            if (editingMessageId === message.id) {
                handleCancelEdit()
            }
        } catch (error) {
            logError('Failed to delete private message', error)
            setMessageActionError('Could not delete the message. Please try again')
        }
    }

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

            {messageActionError && (
                <p className="private-conversation__error" role="alert">
                    {messageActionError}
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

                                    {editingMessageId === message.id ? (
                                        <form
                                            className="private-message__edit-form"
                                            onSubmit={(event) => {
                                                event.preventDefault()
                                                void handleSaveEdit(message)
                                            }}
                                        >
                                            <textarea
                                                className="private-message__edit-input"
                                                value={editContent}
                                                onChange={(event) => setEditContent(event.target.value)}
                                                rows={3}
                                                maxLength={4000}
                                                autoFocus
                                                aria-label="Edit message"
                                                disabled={updateMessageMutation.isPending}
                                            />

                                            <div className="private-message__actions">
                                                <button
                                                    type="submit"
                                                    disabled={updateMessageMutation.isPending}
                                                >
                                                    {updateMessageMutation.isPending ? 'Saving…' : 'Save'}
                                                </button>

                                                <button
                                                    type="button"
                                                    onClick={handleCancelEdit}
                                                    disabled={updateMessageMutation.isPending}
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
                                                        onClick={() => handleStartEdit(message)}
                                                        disabled={deleteMessageMutation.isPending}
                                                    >
                                                        Edit
                                                    </button>

                                                    <button
                                                        type="button"
                                                        onClick={() => void handleDeleteMessage(message)}
                                                        disabled={deleteMessageMutation.isPending}
                                                    >
                                                        {deleteMessageMutation.isPending ? 'Deleting…' : 'Delete'}
                                                    </button>
                                                </div>
                                            )}
                                        </>
                                    )}
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