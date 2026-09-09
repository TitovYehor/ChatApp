import {
    useState,
} from 'react'

export function useChatNavigation() {
    const [
        selectedWorkspaceId,
        setSelectedWorkspaceId,
    ] = useState<string | null>(null)

    const [
        selectedChannelId,
        setSelectedChannelId,
    ] = useState<string | null>(null)

    function selectWorkspace(
        workspaceId: string,
    ) {
        setSelectedWorkspaceId(
            workspaceId,
        )

        setSelectedChannelId(
            null,
        )
    }

    function selectChannel(
        channelId: string | null,
    ) {
        setSelectedChannelId(
            channelId,
        )
    }

    function selectCreatedWorkspace(
        workspaceId: string,
    ) {
        setSelectedWorkspaceId(
            workspaceId,
        )

        setSelectedChannelId(
            null,
        )
    }

    function selectCreatedChannel(
        channelId: string,
    ) {
        setSelectedChannelId(
            channelId,
        )
    }

    function clearSelectedChannel(
        channelId: string,
    ) {
        if (
            selectedChannelId !==
            channelId
        ) {
            return
        }

        setSelectedChannelId(
            null,
        )
    }

    function clearSelectedWorkspace(
        workspaceId: string,
    ) {
        if (
            selectedWorkspaceId !==
            workspaceId
        ) {
            return
        }

        setSelectedWorkspaceId(
            null,
        )

        setSelectedChannelId(
            null,
        )
    }

    function clearCurrentWorkspace() {
        setSelectedWorkspaceId(
            null,
        )

        setSelectedChannelId(
            null,
        )
    }

    return {
        selectedWorkspaceId,
        selectedChannelId,

        selectWorkspace,
        selectChannel,

        selectCreatedWorkspace,
        selectCreatedChannel,

        clearSelectedChannel,
        clearSelectedWorkspace,
        clearCurrentWorkspace,
    }
}