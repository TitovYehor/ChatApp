import ChannelCreateForm from './ChannelCreateForm'

import type {
    ChannelResponse,
} from '../../types/channelTypes'

interface ChannelSidebarProps {
    channels: ChannelResponse[]
    selectedChannelId: string | null

    canManageChannels: boolean

    isCreating: boolean
    createError: string | null

    onSelectChannel: (
        channelId: string,
    ) => void

    onCreateChannel: (
        name: string,
    ) => Promise<void>
}

function ChannelSidebar({
    channels,
    selectedChannelId,
    canManageChannels,
    isCreating,
    createError,
    onSelectChannel,
    onCreateChannel,
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
                        ) => {
                            const isSelected =
                                channel.id ===
                                selectedChannelId

                            return (
                                <li
                                    key={
                                        channel.id
                                    }
                                >
                                    <button
                                        type="button"
                                        onClick={() =>
                                            onSelectChannel(
                                                channel.id,
                                            )
                                        }
                                        aria-pressed={
                                            isSelected
                                        }
                                    >
                                        #
                                        {' '}
                                        {
                                            channel.name
                                        }
                                    </button>
                                </li>
                            )
                        },
                    )}
                </ul>
            )}
        </div>
    )
}

export default ChannelSidebar