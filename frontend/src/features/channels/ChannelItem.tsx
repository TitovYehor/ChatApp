import {
    useState,
} from 'react'

import type {
    ChannelResponse,
} from '../../types/channelTypes'

interface ChannelItemProps {
    channel: ChannelResponse
    isSelected: boolean
    canManageChannels: boolean

    isUpdating: boolean
    updateChannelError: string | null

    onSelect: (
        channelId: string,
    ) => void

    onUpdate: (
        channelId: string,
        name: string,
    ) => Promise<void>
}

function ChannelItem({
    channel,
    isSelected,
    canManageChannels,
    isUpdating,
    updateChannelError,
    onSelect,
    onUpdate,
}: ChannelItemProps) {
    const [
        isEditing,
        setIsEditing,
    ] = useState(false)

    const [
        editingName,
        setEditingName,
    ] = useState(channel.name)

    function handleStartEditing() {
        setEditingName(channel.name)
        setIsEditing(true)
    }

    function handleCancelEditing() {
        setEditingName(channel.name)
        setIsEditing(false)
    }

    async function handleSaveEditing() {
        const name = editingName.trim()

        if (!name) {
            return
        }

        await onUpdate(
            channel.id,
            name,
        )

        setIsEditing(false)
    }

    return (
        <li>
            {isEditing ? (
                <>
                    <input
                        type="text"
                        value={editingName}
                        onChange={(
                            event,
                        ) =>
                            setEditingName(
                                event.target.value,
                            )
                        }
                        maxLength={100}
                        disabled={
                            isUpdating
                        }
                    />

                    <button
                        type="button"
                        onClick={() => {
                            void handleSaveEditing()
                        }}
                        disabled={
                            isUpdating ||
                            editingName
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

                    {updateChannelError && (
                        <p>
                            {
                                updateChannelError
                            }
                        </p>
                    )}
                </>
            ) : (
                <>
                    <button
                        type="button"
                        onClick={() =>
                            onSelect(
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

                    {canManageChannels && (
                        <button
                            type="button"
                            onClick={
                                handleStartEditing
                            }
                            disabled={
                                isUpdating
                            }
                        >
                            Edit
                        </button>
                    )}
                </>
            )}
        </li>
    )
}

export default ChannelItem