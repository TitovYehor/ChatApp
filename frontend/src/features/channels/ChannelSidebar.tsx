import type { ChannelResponse } from '../../types/channelTypes'

interface ChannelSidebarProps {
    channels: ChannelResponse[]
    selectedChannelId: string | null
    onSelectChannel: (
        channelId: string,
    ) => void
}

function ChannelSidebar({
    channels,
    selectedChannelId,
    onSelectChannel,
}: ChannelSidebarProps) {
    return (
        <div>
            <h2>Channels</h2>

            {channels.length === 0 ? (
                <p>No channels</p>
            ) : (
                <ul>
                    {channels.map((channel) => {
                        const isSelected =
                            channel.id ===
                            selectedChannelId

                        return (
                            <li key={channel.id}>
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
                                    # {channel.name}
                                </button>
                            </li>
                        )
                    })}
                </ul>
            )}
        </div>
    )
}

export default ChannelSidebar