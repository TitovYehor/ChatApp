import { useState } from 'react'

import { useChannels } from '../features/channels/useChannels'
import { useWorkspaces } from '../features/workspaces/useWorkspaces'

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

    const {
        channels,
        isLoading: isLoadingChannels,
        error: channelsError,
    } = useChannels(selectedWorkspaceId)

    if (isLoadingWorkspaces) {
        return <div>Loading workspaces...</div>
    }

    if (workspacesError) {
        return <div>{workspacesError}</div>
    }

    return (
        <main>
            <h1>Chat</h1>

            <section>
                <h2>Workspaces</h2>

                {workspaces.length === 0 ? (
                    <p>No workspaces found</p>
                ) : (
                    <ul>
                        {workspaces.map((workspace) => (
                            <li key={workspace.id}>
                                <button
                                    type="button"
                                    onClick={() =>
                                        setSelectedWorkspaceId(
                                            workspace.id,
                                        )
                                    }
                                >
                                    {workspace.name}
                                </button>
                            </li>
                        ))}
                    </ul>
                )}
            </section>

            <section>
                <h2>Channels</h2>

                {selectedWorkspaceId === null ? (
                    <p>
                        Select a workspace to see its channels
                    </p>
                ) : isLoadingChannels ? (
                    <p>Loading channels...</p>
                ) : channelsError ? (
                    <p>{channelsError}</p>
                ) : channels.length === 0 ? (
                    <p>No channels found</p>
                ) : (
                    <ul>
                        {channels.map((channel) => (
                            <li key={channel.id}>
                                {channel.name}
                            </li>
                        ))}
                    </ul>
                )}
            </section>
        </main>
    )
}

export default ChatPage