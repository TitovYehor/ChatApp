import React, { useState, useRef, useEffect } from 'react'

import ChatLayout from '../layouts/ChatLayout'

import WorkspaceSidebar from '../features/workspaces/WorkspaceSidebar'
import ChannelSidebar from '../features/channels/ChannelSidebar'

import { useAuth } from '../features/auth/useAuth'
import { useWorkspaces } from '../features/workspaces/useWorkspaces'
import { useChannels } from '../features/channels/useChannels'
import { useMessages } from '../features/messages/useMessages'
import { useChannelSignalR } from '../features/chat/useChannelSignalR'
import { useRealtimeMessages } from '../features/messages/useRealtimeMessages'
import { useWorkspaceMembers } from '../features/workspaces/useWorkspaceMembers'
import WorkspaceMembers from '../features/workspaces/WorkspaceMembers'
import { usePresence } from '../features/presence/usePresence'
import { useTypingIndicator } from '../features/presence/useTypingIndicator'

function ChatPage() {
    const {
        user,
        logout,
    } = useAuth()

    const {
        workspaces,
        isLoading: isLoadingWorkspaces,
        error: workspacesError,
    } = useWorkspaces()

    const [
        selectedWorkspaceId,
        setSelectedWorkspaceId,
    ] = useState<string | null>(null)

    const [
        selectedChannelId,
        setSelectedChannelId,
    ] = useState<string | null>(null)

    const {
        channels,
        isLoading: isLoadingChannels,
        error: channelsError,
    } = useChannels(selectedWorkspaceId)

    const {
        messages,
        isLoading: isMessagesLoading,
        error: messagesError,
        sendMessage,
        isSending,
        sendError,
        updateMessage,
        isUpdating,
        updateError,
        deleteMessage,
        isDeleting,
        deleteError,
    } = useMessages(selectedChannelId)

    const [messageContent, setMessageContent] = useState('')

    const [
        editingMessageId,
        setEditingMessageId,
    ] = useState<string | null>(null)

    const [
        editingMessageContent,
        setEditingMessageContent,
    ] = useState('')

    useChannelSignalR(selectedChannelId)

    useRealtimeMessages(selectedChannelId)

    const {
        members,
        isLoading: isLoadingMembers,
        error: membersError,
    } = useWorkspaceMembers(
        selectedWorkspaceId,
    )

    const {
        onlineUsers,
    } = usePresence()

    const {
        typingUsers,
        startTyping,
        stopTyping,
    } = useTypingIndicator(
        selectedChannelId,
        user?.id ?? null,
        )

    const typingTimeoutRef =
        useRef<ReturnType<typeof setTimeout> | null>(
            null,
        )

    useEffect(() => {
        if (typingTimeoutRef.current) {
            clearTimeout(
                typingTimeoutRef.current,
            )

            typingTimeoutRef.current = null
        }

        void stopTyping()

    }, [
        selectedChannelId,
        stopTyping,
    ])

    const handleSelectWorkspace = (
        workspaceId: string,
    ) => {
        setSelectedWorkspaceId(workspaceId)
        setSelectedChannelId(null)
    }

    if (isLoadingWorkspaces) {
        return <div>Loading workspaces...</div>
    }

    if (workspacesError) {
        return <div>{workspacesError}</div>
    }

    return (
        <ChatLayout
            workspaces={
                <WorkspaceSidebar
                    workspaces={workspaces}
                    selectedWorkspaceId={
                        selectedWorkspaceId
                    }
                    onSelectWorkspace={
                        handleSelectWorkspace
                    }
                />
            }
            channels={
                selectedWorkspaceId === null ? (
                    <p>Select a workspace</p>
                ) : isLoadingChannels ? (
                    <p>Loading channels...</p>
                ) : channelsError ? (
                    <p>{channelsError}</p>
                ) : (
                    <>
                        <ChannelSidebar
                            channels={channels}
                            selectedChannelId={
                                selectedChannelId
                            }
                            onSelectChannel={
                                setSelectedChannelId
                            }
                        />

                        {isLoadingMembers ? (
                            <p>Loading members...</p>
                        ) : membersError ? (
                            <p>{membersError}</p>
                        ) : (
                            <WorkspaceMembers
                                members={members}
                                onlineUsers={onlineUsers}
                            />
                        )}
                    </>
                )
            }
        >
            <div>
                <p>
                    Logged in as {user?.username}
                </p>

                <button
                    type="button"
                    onClick={logout}
                >
                    Logout
                </button>
            </div>

            {selectedChannelId === null ? (
                <div>
                    <h1>Chat</h1>
                    <p>
                        Select a channel to start chatting
                    </p>
                </div>
            ) : (
                <section>
                    <h2>Messages</h2>

                    {!selectedChannelId ? (
                        <p>Select a channel</p>
                    ) : isMessagesLoading ? (
                        <p>Loading messages...</p>
                    ) : messagesError ? (
                        <p>{messagesError}</p>
                    ) : messages.length === 0 ? (
                        <p>No messages yet</p>
                    ) : (
                        <ul>
                            {messages.map((message) => {
                                const isEditing =
                                    editingMessageId ===
                                    message.id

                                const isOwnMessage =
                                    message.userId ===
                                    user?.id

                                return (
                                    <li key={message.id}>
                                        {isEditing ? (
                                            <>
                                                <input
                                                    type="text"
                                                    value={
                                                        editingMessageContent
                                                    }
                                                    onChange={(
                                                        event,
                                                    ) =>
                                                        setEditingMessageContent(
                                                            event
                                                                .target
                                                                .value,
                                                        )
                                                    }
                                                    disabled={
                                                        isUpdating
                                                    }
                                                />

                                                <button
                                                    type="button"
                                                    onClick={() =>
                                                        void handleSaveEditing()
                                                    }
                                                    disabled={
                                                        isUpdating ||
                                                        editingMessageContent
                                                            .trim()
                                                            .length ===
                                                        0
                                                    }
                                                >
                                                    {isUpdating
                                                        ? 'Saving...'
                                                        : 'Save'}
                                                </button>

                                                <button
                                                    type="button"
                                                    onClick={
                                                        handleCancelEditing
                                                    }
                                                    disabled={
                                                        isUpdating
                                                    }
                                                >
                                                    Cancel
                                                </button>
                                            </>
                                        ) : (
                                            <>
                                                <strong>
                                                    {
                                                        message.username
                                                    }
                                                </strong>
                                                :{' '}
                                                {
                                                    message.content
                                                }

                                                {message.updatedAt && (
                                                    <span>
                                                        {' '}
                                                        (edited)
                                                    </span>
                                                )}

                                                {isOwnMessage && (
                                                    <>
                                                        <button
                                                            type="button"
                                                            onClick={() =>
                                                                handleStartEditing(
                                                                    message.id,
                                                                    message.content,
                                                                )
                                                            }
                                                            disabled={
                                                                isUpdating ||
                                                                isDeleting
                                                            }
                                                        >
                                                            Edit
                                                        </button>

                                                        <button
                                                            type="button"
                                                            onClick={() =>
                                                                void handleDeleteMessage(
                                                                    message.id,
                                                                )
                                                            }
                                                            disabled={
                                                                isDeleting ||
                                                                isUpdating
                                                            }
                                                        >
                                                            Delete
                                                        </button>
                                                    </>
                                                )}
                                            </>
                                        )}
                                    </li>
                                )
                            })}
                        </ul>
                    )}

                    {updateError && (
                        <p>{updateError}</p>
                    )}

                    {deleteError && (
                        <p>{deleteError}</p>
                    )}

                    {typingUsers.length > 0 && (
                        <p>
                            {typingUsers.length === 1
                                ? `${typingUsers[0].username} is typing...`
                                : typingUsers.length === 2
                                    ? `${typingUsers[0].username} and ${typingUsers[1].username} are typing...`
                                    : `${typingUsers[0].username} and ${typingUsers.length - 1} others are typing...`}
                        </p>
                    )}

                    {selectedChannelId && (
                        <form onSubmit={handleSendMessage}>
                            <input
                                type="text"
                                value={messageContent}
                                onChange={(event) => {
                                    const value =
                                        event.target.value

                                    setMessageContent(value)

                                    if (!value.trim()) {
                                        void stopTyping()
                                        return
                                    }

                                    void startTyping()

                                    if (typingTimeoutRef.current) {
                                        clearTimeout(
                                            typingTimeoutRef.current,
                                        )
                                    }

                                    typingTimeoutRef.current =
                                        setTimeout(() => {
                                            void stopTyping()
                                        }, 1500)
                                }}
                                placeholder="Write a message..."
                                disabled={isSending}
                            />

                            <button
                                type="submit"
                                disabled={
                                    isSending ||
                                    messageContent.trim().length === 0
                                }
                            >
                                {isSending
                                    ? 'Sending...'
                                    : 'Send'}
                            </button>

                            {sendError && (
                                <p>{sendError}</p>
                            )}
                        </form>
                    )}
                </section>
            )}
        </ChatLayout>
    )

    function handleSendMessage(
        event: React.SubmitEvent,
    ) {
        event.preventDefault()

        const content = messageContent.trim()

        if (!selectedChannelId || !content) {
            return
        }

        if (typingTimeoutRef.current) {
            clearTimeout(
                typingTimeoutRef.current,
            )

            typingTimeoutRef.current = null
        }

        void stopTyping()

        void sendMessage(content).then(() => {
            setMessageContent('')
        })
    }

    function handleStartEditing(
        messageId: string,
        content: string,
    ) {
        setEditingMessageId(messageId)
        setEditingMessageContent(content)
    }

    function handleCancelEditing() {
        setEditingMessageId(null)
        setEditingMessageContent('')
    }

    async function handleSaveEditing() {
        if (!editingMessageId) {
            return
        }

        const content = editingMessageContent.trim()

        if (!content) {
            return
        }

        await updateMessage({
            messageId:
                editingMessageId,
            content,
        })

        setEditingMessageId(null)
        setEditingMessageContent('')
    }

    async function handleDeleteMessage(
        messageId: string,
    ) {
        const confirmed = window.confirm(
            'Delete this message?',
        )

        if (!confirmed) {
            return
        }

        await deleteMessage(messageId)
    }
}

export default ChatPage