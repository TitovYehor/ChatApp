import {
    useEffect,
    useState,
} from 'react'

import ChatLayout from '../layouts/ChatLayout'

import WorkspaceSidebar from '../features/workspaces/WorkspaceSidebar'
import ChannelSidebar from '../features/channels/ChannelSidebar'
import WorkspaceMembers from '../features/workspaces/WorkspaceMembers'

import MessageList from '../features/messages/MessageList'
import MessageComposer from '../features/messages/MessageComposer'
import TypingIndicator from '../features/presence/TypingIndicator'

import { useAuth } from '../features/auth/useAuth'
import { useWorkspaces } from '../features/workspaces/useWorkspaces'
import { useChannels } from '../features/channels/useChannels'
import { useMessages } from '../features/messages/useMessages'
import { useChannelSignalR } from '../features/chat/useChannelSignalR'
import { useRealtimeMessages } from '../features/messages/useRealtimeMessages'
import { useWorkspaceMembers } from '../features/workspaces/useWorkspaceMembers'
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
    } = useChannels(
        selectedWorkspaceId,
    )

    const {
        messages,
        isLoading:
        isMessagesLoading,
        error: messagesError,
        sendMessage,
        isSending,
        sendError,
        updateMessage,
        updatingMessageId,
        deleteMessage,
        deletingMessageId,
    } = useMessages(
        selectedChannelId,
    )

    useChannelSignalR(
        selectedChannelId,
    )

    useRealtimeMessages(
        selectedChannelId,
    )

    const {
        members,
        isLoading:
        isLoadingMembers,
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

    useEffect(() => {
        if (!selectedChannelId) {
            return
        }

        return () => {
            void stopTyping()
        }
    }, [
        selectedChannelId,
        stopTyping,
    ])

    function handleSelectWorkspace(
        workspaceId: string,
    ) {
        setSelectedWorkspaceId(
            workspaceId,
        )

        setSelectedChannelId(null)
    }

    async function handleSendMessage(
        content: string,
    ) {
        await sendMessage(content)
    }

    async function handleUpdateMessage(
        messageId: string,
        content: string,
    ) {
        await updateMessage({
            messageId,
            content,
        })
    }

    async function handleDeleteMessage(
        messageId: string,
    ) {
        await deleteMessage(
            messageId,
        )
    }

    if (isLoadingWorkspaces) {
        return (
            <div>
                Loading workspaces...
            </div>
        )
    }

    if (workspacesError) {
        return (
            <div>
                {workspacesError}
            </div>
        )
    }

    return (
        <ChatLayout
            workspaces={
                <WorkspaceSidebar
                    workspaces={
                        workspaces
                    }
                    selectedWorkspaceId={
                        selectedWorkspaceId
                    }
                    onSelectWorkspace={
                        handleSelectWorkspace
                    }
                />
            }
            channels={
                selectedWorkspaceId ===
                    null ? (
                    <p>
                        Select a workspace
                    </p>
                ) : isLoadingChannels ? (
                    <p>
                        Loading channels...
                    </p>
                ) : channelsError ? (
                    <p>
                        {
                            channelsError
                        }
                    </p>
                ) : (
                    <>
                        <ChannelSidebar
                            channels={
                                channels
                            }
                            selectedChannelId={
                                selectedChannelId
                            }
                            onSelectChannel={
                                setSelectedChannelId
                            }
                        />

                        {isLoadingMembers ? (
                            <p>
                                Loading members...
                            </p>
                        ) : membersError ? (
                            <p>
                                {
                                    membersError
                                }
                            </p>
                        ) : (
                            <WorkspaceMembers
                                members={
                                    members
                                }
                                onlineUsers={
                                    onlineUsers
                                }
                            />
                        )}
                    </>
                )
            }
        >
            <div>
                <p>
                    Logged in as{' '}
                    {
                        user?.username
                    }
                </p>

                <button
                    type="button"
                    onClick={
                        logout
                    }
                >
                    Logout
                </button>
            </div>

            {selectedChannelId ===
                null ? (
                <div>
                    <h1>
                        Chat
                    </h1>

                    <p>
                        Select a
                        channel to
                        start
                        chatting
                    </p>
                </div>
            ) : (
                <section>
                    <h2>
                        Messages
                    </h2>

                    {isMessagesLoading ? (
                        <p>
                            Loading messages...
                        </p>
                    ) : messagesError ? (
                        <p>
                            {
                                messagesError
                            }
                        </p>
                    ) : messages.length ===
                        0 ? (
                        <p>
                            No messages
                            yet
                        </p>
                    ) : (
                        <MessageList
                            messages={messages}
                            currentUserId={
                                user?.id ?? null
                            }
                            updatingMessageId={
                                updatingMessageId
                            }
                            deletingMessageId={
                                deletingMessageId
                            }
                            onUpdate={
                                handleUpdateMessage
                            }
                            onDelete={
                                handleDeleteMessage
                            }
                        />
                    )}

                    <TypingIndicator
                        typingUsers={
                            typingUsers
                        }
                    />

                    <MessageComposer
                        isSending={
                            isSending
                        }
                        sendError={
                            sendError
                        }
                        onSend={
                            handleSendMessage
                        }
                        onStartTyping={
                            startTyping
                        }
                        onStopTyping={
                            stopTyping
                        }
                    />
                </section>
            )}
        </ChatLayout>
    )
}

export default ChatPage