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
} from '../../types/channelTypes'

export function useRealtimeChannels(
    workspaceId: string | null,
) {
    const queryClient =
        useQueryClient()

    const connection =
        getChatConnection()

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

        connection.on(
            SignalREvents.ChannelCreated,
            handleChannelCreated,
        )

        return () => {
            connection.off(
                SignalREvents.ChannelCreated,
                handleChannelCreated,
            )
        }
    }, [
        connection,
        queryClient,
        workspaceId,
    ])
}