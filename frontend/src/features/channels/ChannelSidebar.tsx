import ChannelCreateForm from './ChannelCreateForm'
import ChannelItem from './ChannelItem'

import type {
    ChannelResponse,
} from '../../types/channelTypes'

interface ChannelSidebarProps {
    channels: ChannelResponse[]
    selectedChannelId: string | null

    canManageChannels: boolean

    isCreating: boolean
    createError: string | null

    updatingChannelId: string | null
    updateChannelError: string | null
    updateErrorChannelId: string | null

    onSelectChannel: (
        channelId: string,
    ) => void

    onCreateChannel: (
        name: string,
    ) => Promise<void>

    onUpdateChannel: (
        channelId: string,
        name: string,
    ) => Promise<void>
}

function ChannelSidebar({
    channels,
    selectedChannelId,
    canManageChannels,
    isCreating,
    createError,
    updatingChannelId,
    updateChannelError,
    updateErrorChannelId,
    onSelectChannel,
    onCreateChannel,
    onUpdateChannel,
}: ChannelSidebarProps) {
    return (
        <div>
            <h2>
                Channels
            </h2>

            {canManageChannels && (
                <ChannelCreateForm
                    isCreating={
                        isCreating
                    }
                    createError={
                        createError
                    }
                    onCreate={
                        onCreateChannel
                    }
                />
            )}

            {channels.length ===
                0 ? (
                <p>
                    No channels
                </p>
            ) : (
                <ul>
                    {channels.map(
                        (
                            channel,
                        ) => (
                            <ChannelItem
                                key={
                                    channel.id
                                }
                                channel={
                                    channel
                                }
                                isSelected={
                                    channel.id ===
                                    selectedChannelId
                                }
                                canManageChannels={
                                    canManageChannels
                                }
                                isUpdating={
                                    updatingChannelId ===
                                    channel.id
                                }
                                updateChannelError={
                                    updateErrorChannelId ===
                                        channel.id
                                        ? updateChannelError
                                        : null
                                }
                                onSelect={
                                    onSelectChannel
                                }
                                onUpdate={
                                    onUpdateChannel
                                }
                            />
                        ),
                    )}
                </ul>
            )}
        </div>
    )
}

export default ChannelSidebar