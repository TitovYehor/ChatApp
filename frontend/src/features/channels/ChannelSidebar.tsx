import ChannelCreateForm from './ChannelCreateForm'
import ChannelItem from './ChannelItem'

import './css/ChannelSidebar.css'

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

    deletingChannelId: string | null
    deleteChannelError: string | null
    deleteErrorChannelId: string | null

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

    onDeleteChannel: (
        channelId: string,
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
    deletingChannelId,
    deleteChannelError,
    deleteErrorChannelId,
    onSelectChannel,
    onCreateChannel,
    onUpdateChannel,
    onDeleteChannel,
}: ChannelSidebarProps) {
    return (
        <div className="channel-sidebar">
            <div className="channel-sidebar__header">
                <h2>
                    Channels
                </h2>
            </div>

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
                <p className="channel-sidebar__empty">
                    No channels
                </p>
            ) : (
                <ul className="channel-sidebar__list">
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
                                isDeleting={
                                    deletingChannelId ===
                                    channel.id
                                }
                                deleteChannelError={
                                    deleteErrorChannelId ===
                                        channel.id
                                        ? deleteChannelError
                                        : null
                                }
                                onSelect={
                                    onSelectChannel
                                }
                                onUpdate={
                                    onUpdateChannel
                                }
                                onDelete={
                                    onDeleteChannel
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