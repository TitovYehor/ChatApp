import { useState } from 'react'

import ChatLayout from '../layouts/ChatLayout'

import WorkspaceSidebar from '../features/workspaces/WorkspaceSidebar'
import ChannelSidebar from '../features/channels/ChannelSidebar'

import { useWorkspaces } from '../features/workspaces/useWorkspaces'
import { useChannels } from '../features/channels/useChannels'
import { useMessages } from '../features/messages/useMessages'
import { useChatConnection } from '../features/chat/useChatConnection'
import { useChannelSignalR } from '../features/chat/useChannelSignalR'
import { useRealtimeMessages } from '../features/messages/useRealtimeMessages'

function ChatPage() {
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
    } = useMessages(selectedChannelId)

    const [messageContent, setMessageContent] = useState('')

    const {
        isConnected,
        error: connectionError,
    } = useChatConnection()

    useChannelSignalR(selectedChannelId)

    useRealtimeMessages(selectedChannelId)

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
                    <ChannelSidebar
                        channels={channels}
                        selectedChannelId={
                            selectedChannelId
                        }
                        onSelectChannel={
                            setSelectedChannelId
                        }
                    />
                )
            }
        >
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
                            {messages.map((message) => (
                                <li key={message.id}>
                                    <strong>
                                        {message.username}
                                    </strong>
                                    : {message.content}
                                </li>
                            ))}
                        </ul>
                    )}

                    {selectedChannelId && (
                        <form onSubmit={handleSendMessage}>
                            <input
                                type="text"
                                value={messageContent}
                                onChange={(event) =>
                                    setMessageContent(
                                        event.target.value,
                                    )
                                }
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

            {connectionError && (
                <p>{connectionError}</p>
            )}

            <p>
                Realtime:{' '}
                {isConnected
                    ? 'Connected'
                    : 'Connecting...'}
            </p>
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

        void sendMessage(content).then(() => {
            setMessageContent('')
        })
    }
}

export default ChatPage