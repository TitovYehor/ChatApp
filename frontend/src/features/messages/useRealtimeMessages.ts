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

import type {
    PagedResult,
} from '../../types/pagedResultType'

export function useRealtimeMessages(
    channelId: string | null,
) {
    const queryClient =
        useQueryClient()

    const connection =
        getChatConnection()

    useEffect(() => {
        if (!channelId) {
            return
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

            queryClient.setQueryData<
                PagedResult<MessageResponse>
            >(
                [
                    'messages',
                    channelId,
                ],
                (current) => {
                    if (!current) {
                        return current
                    }

                    const alreadyExists =
                        current.items.some(
                            (item) =>
                                item.id ===
                                message.id,
                        )

                    if (alreadyExists) {
                        return current
                    }

                    return {
                        ...current,
                        items: [
                            ...current.items,
                            message,
                        ],
                        totalCount:
                            current.totalCount +
                            1,
                    }
                },
            )
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

            queryClient.setQueryData<
                PagedResult<MessageResponse>
            >(
                [
                    'messages',
                    channelId,
                ],
                (current) => {
                    if (!current) {
                        return current
                    }

                    return {
                        ...current,
                        items:
                            current.items.map(
                                (item) =>
                                    item.id ===
                                        message.id
                                        ? message
                                        : item,
                            ),
                    }
                },
            )
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

            queryClient.setQueryData<
                PagedResult<MessageResponse>
            >(
                [
                    'messages',
                    channelId,
                ],
                (current) => {
                    if (!current) {
                        return current
                    }

                    const items =
                        current.items.filter(
                            (item) =>
                                item.id !==
                                response.messageId,
                        )

                    if (
                        items.length ===
                        current.items.length
                    ) {
                        return current
                    }

                    return {
                        ...current,
                        items,
                        totalCount:
                            Math.max(
                                0,
                                current.totalCount -
                                1,
                            ),
                    }
                },
            )
        }

        connection.on(
            'MessageCreated',
            handleMessageCreated,
        )

        connection.on(
            'MessageUpdated',
            handleMessageUpdated,
        )

        connection.on(
            'MessageDeleted',
            handleMessageDeleted,
        )

        return () => {
            connection.off(
                'MessageCreated',
                handleMessageCreated,
            )

            connection.off(
                'MessageUpdated',
                handleMessageUpdated,
            )

            connection.off(
                'MessageDeleted',
                handleMessageDeleted,
            )
        }
    }, [
        channelId,
        connection,
        queryClient,
    ])
}