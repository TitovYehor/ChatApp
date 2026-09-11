import {
    useEffect,
} from 'react'

import {
    useQuery,
    useQueryClient,
} from '@tanstack/react-query'

import {
    getChatConnection,
} from '../../services/signalR/chatConnection'

import type {
    OnlineUserResponse,
    UserPresenceChangedResponse,
} from '../../types/presenceTypes'

import {
    SignalREvents
} from '../../types/signalREvents'

const PRESENCE_QUERY_KEY = [
    'presence',
]

export function usePresence() {
    const queryClient =
        useQueryClient()

    const connection =
        getChatConnection()

    const query =
        useQuery<OnlineUserResponse[]>({
            queryKey:
                PRESENCE_QUERY_KEY,

            queryFn: async () => [],

            staleTime:
                Infinity,
        })

    useEffect(() => {
        const handleSnapshot = (
            users: OnlineUserResponse[],
        ) => {
            queryClient.setQueryData(
                PRESENCE_QUERY_KEY,
                users,
            )
        }

        const handlePresenceChanged = (
            response: UserPresenceChangedResponse,
        ) => {
            queryClient.setQueryData<
                OnlineUserResponse[]
            >(
                PRESENCE_QUERY_KEY,
                (current) => {
                    if (!current) {
                        return current
                    }

                    if (response.isOnline) {
                        const exists =
                            current.some(
                                (user) =>
                                    user.userId ===
                                    response.userId,
                            )

                        if (exists) {
                            return current
                        }

                        return [
                            ...current,
                            {
                                userId:
                                    response.userId,
                                username:
                                    response.username,
                            },
                        ]
                    }

                    return current.filter(
                        (user) =>
                            user.userId !==
                            response.userId,
                    )
                },
            )
        }

        connection.on(
            SignalREvents.OnlineUsersSnapshot,
            handleSnapshot,
        )

        connection.on(
            SignalREvents.UserPresenceChanged,
            handlePresenceChanged,
        )

        return () => {
            connection.off(
                SignalREvents.OnlineUsersSnapshot,
                handleSnapshot,
            )

            connection.off(
                SignalREvents.UserPresenceChanged,
                handlePresenceChanged,
            )
        }
    }, [
        connection,
        queryClient,
    ])

    return {
        onlineUsers:
            query.data ?? [],
    }
}