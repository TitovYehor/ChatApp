import ChannelCreateForm from './ChannelCreateForm'
import ChannelItem from './ChannelItem'

import './css/ChannelSidebar.css'

import type {
    ChannelResponse,
} from '../../types/channelTypes'

interface ChannelSidebarProps {
    selectedChannelId: string | null

    searchQuery: string
    searchError: string | null
    displayChannels: ChannelResponse[]

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

    onSearchChange: (
        value: string,
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
    selectedChannelId,
    searchQuery,
    searchError,
    displayChannels,
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
    onSearchChange,
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

                <input
                    type="text"
                    value={searchQuery}
                    onChange={(event) =>
                        onSearchChange(
                            event.target.value,
                        )
                    }
                    placeholder="Search channels..."
                    aria-label="Search channels"
                />
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

            {searchError ? (
                <p className="channel-sidebar__empty">
                    {searchError}
                </p>
            ) : displayChannels.length ===
                0 ? (
                <p className="channel-sidebar__empty">
                    {searchQuery
                        ? 'No matching channels'
                        : 'No channels'}
                </p>
            ) : (
                <ul className="channel-sidebar__list">
                    {displayChannels.map(
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