import {
    useChannels,
} from './useChannels'

export function useChannelController(
    workspaceId: string | null,
) {
    const {
        channels,
        isLoading: isLoadingChannels,
        error: channelsError,

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