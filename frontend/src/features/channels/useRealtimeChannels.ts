import {
    useEffect,
} from 'react'

import {
    useQueryClient,
} from '@tanstack/react-query'

import {
    getChatConnection,
} from '../../services/signalR/chatConnection'

import {
    SignalREvents,
} from '../../types/signalREvents'

import type {
    ChannelResponse,
    ChannelDeletedResponse,
} from '../../types/channelTypes'

export function useRealtimeChannels(
    workspaceId: string | null,
    selectedChannelId: string | null,
    onChannelDeleted: (
        channelId: string,
    ) => void,
) {
    const queryClient = useQueryClient()

    const connection = getChatConnection()

    useEffect(() => {
        if (!workspaceId) return

        const handleChannelCreated = (
            channel: ChannelResponse,
        ) => {
            if (
                channel.workspaceId !==
                workspaceId
            ) {
                return
            }

            queryClient.setQueryData<
                ChannelResponse[]
            >(
                [
                    'channels',
                    workspaceId,
                ],
                (
                    current,
                ) => {
                    if (!current) {
                        return [
                            channel,
                        ]
                    }

                    const alreadyExists =
                        current.some(
                            (
                                item,
                            ) =>
                                item.id ===
                                channel.id,
                        )

                    if (alreadyExists) {
                        return current
                    }

                    return [
                        ...current,
                        channel,
                    ]
                },
            )
        }

        const handleChannelUpdated = (
            channel: ChannelResponse,
        ) => {
            if (
                channel.workspaceId !==
                workspaceId
            ) {
                return
            }

            queryClient.setQueryData<
                ChannelResponse[]
            >(
                [
                    'channels',
                    workspaceId,
                ],
                (
                    current,
                ) => {
                    if (!current) {
                        return current
                    }

                    return current.map(
                        (
                            currentChannel,
                        ) =>
                            currentChannel.id ===
                                channel.id
                                ? channel
                                : currentChannel,
                    )
                },
            )
        }

        const handleChannelDeleted = (
            response: ChannelDeletedResponse,
        ) => {
            if (
                response.workspaceId !==
                workspaceId
            ) {
                return
            }

            queryClient.setQueryData<
                ChannelResponse[]
            >(
                [
                    'channels',
                    workspaceId,
                ],
                (
                    current,
                ) => {
                    if (!current) {
                        return current
                    }

                    return current.filter(
                        (
                            channel,
                        ) =>
                            channel.id !==
                            response.channelId,
                    )
                },
            )

            if (
                selectedChannelId ===
                response.channelId
            ) {
                onChannelDeleted(
                    response.channelId,
                )
            }
        }

        connection.on(
            SignalREvents.ChannelCreated,
            handleChannelCreated,
        )

        connection.on(
            SignalREvents.ChannelUpdated,
            handleChannelUpdated,
        )

        connection.on(
            SignalREvents.ChannelDeleted,
            handleChannelDeleted,
        )

        return () => {
            connection.off(
                SignalREvents.ChannelCreated,
                handleChannelCreated,
            )

            connection.off(
                SignalREvents.ChannelUpdated,
                handleChannelUpdated,
            )

            connection.off(
                SignalREvents.ChannelDeleted,
                handleChannelDeleted,
            )
        }
    }, [
        connection,
        queryClient,
        workspaceId,
        selectedChannelId,
        onChannelDeleted,
    ])
}