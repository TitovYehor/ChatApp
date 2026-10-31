import { useCallback, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import { useAuth } from '../features/auth/useAuth'
import { usePrivateConversations } from '../features/privateConversations/usePrivateConversations'
import {
    usePrivateMessages,
    useSendPrivateMessage,
    useUpdatePrivateMessage,
    useDeletePrivateMessage,
} from '../features/privateConversations/usePrivateMessages'
import PrivateMessageComposer from '../features/privateConversations/PrivateMessageComposer'
import PrivateMessageList from '../features/privateConversations/PrivateMessageList'
import PrivateTypingIndicator from '../features/presence/PrivateTypingIndicator'
import { usePrivateTypingIndicator } from '../features/presence/usePrivateTypingIndicator'
import { usePrivateConversationRealtime } from '../features/privateConversations/usePrivateConversationRealtime'

import type { PrivateMessageResponse } from '../types/privateConversationTypes'

import { logError } from '../utils/logger'

import ChatHeader from '../layouts/ChatHeader'

import './css/PrivateConversationPage.css'

function PrivateConversationPage() {
    const { conversationId } = useParams<{ conversationId: string }>()
    const navigate = useNavigate()
    const { user, logout } = useAuth()

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

    const [sendError, setSendError] = useState<string | null>(null)

    const handleMessagesChanged = useCallback(() => {
        void refetch()
    }, [refetch])

    const {
        liveMessages,
        deletedMessageIds,
        connectionError,
        addLiveMessage,
        upsertLiveMessage,
        removeLiveMessage,
    } = usePrivateConversationRealtime({
        conversationId,
        onMessagesChanged: handleMessagesChanged,
    })

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
    }, [result?.items, liveMessages, deletedMessageIds])

    const handleSend = useCallback(async (content: string) => {
        setSendError(null)

        try {
            const message = await sendMessageMutation.mutateAsync(content)
            addLiveMessage(message)
        } catch (error) {
            logError('Failed to send private message', error)
            setSendError('Could not send your message. Please try again')
            throw error
        }
    }, [sendMessageMutation, addLiveMessage])

    const handleSaveEdit = useCallback(async (
        message: PrivateMessageResponse,
        content: string,
    ) => {
        try {
            const updatedMessage = await updateMessageMutation.mutateAsync({
                messageId: message.id,
                content,
            })

            upsertLiveMessage(updatedMessage)
        } catch (error) {
            logError('Failed to edit private message', error)
            throw error
        }
    }, [updateMessageMutation, upsertLiveMessage])

    const handleDeleteMessage = useCallback(async (
        message: PrivateMessageResponse,
    ) => {
        try {
            await deleteMessageMutation.mutateAsync(message.id)

            if (conversationId) {
                removeLiveMessage(message.id, conversationId)
            }
        } catch (error) {
            logError('Failed to delete private message', error)
            throw error
        }
    }, [deleteMessageMutation, conversationId, removeLiveMessage])

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
        <div className="private-conversation-page">
            <ChatHeader
                username={user?.username ?? 'User'}
                onLogout={() => {
                    void logout()
                }}
            />
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

                <PrivateMessageList
                    messages={messages}
                    currentUserId={user?.id}
                    isLoading={
                        isLoading ||
                        (isLoadingConversations && !conversation)
                    }
                    error={error}
                    isSaving={updateMessageMutation.isPending}
                    isDeleting={deleteMessageMutation.isPending}
                    onRetry={() => void refetch()}
                    onSave={handleSaveEdit}
                    onDelete={handleDeleteMessage}
                />

                <PrivateTypingIndicator typingUsers={typingUsers} />

                <PrivateMessageComposer
                    isSending={sendMessageMutation.isPending}
                    sendError={sendError}
                    onSend={handleSend}
                    onTypingStart={startTyping}
                    onTypingStop={stopTyping}
                />
            </main>
        </div>
    )
}

export default PrivateConversationPage