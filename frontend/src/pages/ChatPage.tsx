import { useState } from 'react'

import ChatLayout from '../layouts/ChatLayout'

import WorkspaceSidebar from '../features/workspaces/WorkspaceSidebar'
import ChannelSidebar from '../features/channels/ChannelSidebar'

import { useWorkspaces } from '../features/workspaces/useWorkspaces'
import { useChannels } from '../features/channels/useChannels'

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
                <div>
                    <h1>Channel selected</h1>
                    <p>
                        Channel ID:{' '}
                        {selectedChannelId}
                    </p>
                </div>
            )}
        </ChatLayout>
    )
}

export default ChatPage