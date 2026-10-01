import {
    useChannels,
} from './useChannels'

import {
    useRealtimeChannels,
} from './useRealtimeChannels'

export function useChannelController(
    workspaceId: string | null,
    selectedChannelId: string | null,
    onChannelDeleted: (
        channelId: string,
    ) => void,
    searchQuery: string,
) {
    useRealtimeChannels(
        workspaceId,
        selectedChannelId,
        onChannelDeleted,
    )

    const {
        channels,
        isLoading: isLoadingChannels,
        error: channelsError,

        searchResults,
        isSearching,
        searchError,

        createChannel,
        isCreating,
        createError,

        updateChannel,
        updatingChannelId,
        updateChannelError,
        updateErrorChannelId,

        deleteChannel,
        deletingChannelId,
        deleteChannelError,
        deleteErrorChannelId,
    } = useChannels(
        workspaceId,
        searchQuery,
    )

    async function createChannelAndReturn(
        name: string,
    ) {
        return createChannel(
            name,
        )
    }

    async function updateChannelDetails(
        channelId: string,
        name: string,
    ) {
        await updateChannel({
            channelId,
            name,
        })
    }

    async function deleteChannelById(
        channelId: string,
    ) {
        await deleteChannel(
            channelId,
        )
    }

    return {
        channels,
        isLoadingChannels,
        channelsError,

        searchResults,
        isSearching,
        searchError,

        createChannelAndReturn,
        isCreating,
        createError,

        updateChannelDetails,
        updatingChannelId,
        updateChannelError,
        updateErrorChannelId,

        deleteChannelById,
        deletingChannelId,
        deleteChannelError,
        deleteErrorChannelId,
    }
}