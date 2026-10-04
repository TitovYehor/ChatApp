import {
    useEffect,
} from 'react'

import {
    useQueryClient,
} from '@tanstack/react-query'

import {
    getChatConnection,
} from '../../services/signalR/chatConnection'

import type {
    MessageResponse,
    MessageDeletedResponse,
} from '../../types/messageTypes'

import {
    SignalREvents,
} from '../../types/signalREvents'

export function useRealtimeMessages(
    channelId: string | null,
) {
    const queryClient = useQueryClient()

    const connection = getChatConnection()

    useEffect(() => {
        if (!channelId) {
            return
        }

        const invalidateChannelMessages = () => {
            void queryClient.invalidateQueries({
                queryKey: [
                    'messages',
                    channelId,
                ],
            })
        }

        const handleMessageCreated = (
            message: MessageResponse,
        ) => {
            if (
                message.channelId !==
                channelId
            ) {
                return
            }

            invalidateChannelMessages()
        }

        const handleMessageUpdated = (
            message: MessageResponse,
        ) => {
            if (
                message.channelId !==
                channelId
            ) {
                return
            }

            invalidateChannelMessages()
        }

        const handleMessageDeleted = (
            response: MessageDeletedResponse,
        ) => {
            if (
                response.channelId !==
                channelId
            ) {
                return
            }

            invalidateChannelMessages()
        }

        connection.on(
            SignalREvents.MessageCreated,
            handleMessageCreated,
        )

        connection.on(
            SignalREvents.MessageUpdated,
            handleMessageUpdated,
        )

        connection.on(
            SignalREvents.MessageDeleted,
            handleMessageDeleted,
        )

        return () => {
            connection.off(
                SignalREvents.MessageCreated,
                handleMessageCreated,
            )

            connection.off(
                SignalREvents.MessageUpdated,
                handleMessageUpdated,
            )

            connection.off(
                SignalREvents.MessageDeleted,
                handleMessageDeleted,
            )
        }
    }, [
        channelId,
        connection,
        queryClient,
    ])
}