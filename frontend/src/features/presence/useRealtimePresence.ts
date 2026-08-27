import { useEffect } from 'react'

import { useQueryClient } from '@tanstack/react-query'

import {
    getChatConnection,
} from '../../services/signalR/chatConnection'

import type {
    OnlineUserResponse,
    UserPresenceChangedResponse,
} from '../../types/presenceTypes'

export function useRealtimePresence() {
    const queryClient =
        useQueryClient()

    const connection =
        getChatConnection()

    useEffect(() => {
        const handlePresenceChanged = (
            response: UserPresenceChangedResponse,
        ) => {
            queryClient.setQueryData<
                OnlineUserResponse[]
            >(
                ['presence', 'online-users'],
                (current) => {
                    const users =
                        current ?? []

                    if (response.isOnline) {
                        const alreadyExists =
                            users.some(
                                (user) =>
                                    user.userId ===
                                    response.userId,
                            )

                        if (alreadyExists) {
                            return users.map(
                                (user) =>
                                    user.userId ===
                                        response.userId
                                        ? {
                                            ...user,
                                            username:
                                                response.username,
                                        }
                                        : user,
                            )
                        }

                        return [
                            ...users,
                            {
                                userId:
                                    response.userId,
                                username:
                                    response.username,
                            },
                        ]
                    }

                    return users.filter(
                        (user) =>
                            user.userId !==
                            response.userId,
                    )
                },
            )
        }

        const handleOnlineUsersSnapshot = (
            users: OnlineUserResponse[],
        ) => {
            queryClient.setQueryData(
                [
                    'presence',
                    'online-users',
                ],
                users,
            )
        }

        connection.on(
            'UserPresenceChanged',
            handlePresenceChanged,
        )

        connection.on(
            'OnlineUsersSnapshot',
            handleOnlineUsersSnapshot,
        )

        return () => {
            connection.off(
                'UserPresenceChanged',
                handlePresenceChanged,
            )

            connection.off(
                'OnlineUsersSnapshot',
                handleOnlineUsersSnapshot,
            )
        }
    }, [
        connection,
        queryClient,
    ])
}