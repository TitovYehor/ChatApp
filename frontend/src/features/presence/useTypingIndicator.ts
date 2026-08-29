import {
    useCallback,
    useEffect,
    useRef,
    useState,
} from 'react'

import {
    getChatConnection,
} from '../../services/signalR/chatConnection'

import type {
    UserTypingResponse,
} from '../../types/presenceTypes'

export function useTypingIndicator(
        channelId: string | null,
        currentUserId: string | null,
    ) {
        const connection =
            getChatConnection()

    const [
        typingUsers,
        setTypingUsers,
    ] = useState<UserTypingResponse[]>([])

    const stopTimers =
        useRef<
            Map<string, ReturnType<typeof setTimeout>>
        >(new Map())

    useEffect(() => {
        const timers = stopTimers.current

        if (!channelId) {
            return
        }

        const handleUserTyping = (
            response: UserTypingResponse,
        ) => {
            if (
                response.channelId !==
                channelId
            ) {
                return
            }

            if (
                response.userId ===
                currentUserId
            ) {
                return
            }

            if (response.isTyping) {
                setTypingUsers((current) => {
                    const exists =
                        current.some(
                            (user) =>
                                user.userId ===
                                response.userId,
                        )

                    if (exists) {
                        return current.map(
                            (user) =>
                                user.userId ===
                                    response.userId
                                    ? response
                                    : user,
                        )
                    }

                    return [
                        ...current,
                        response,
                    ]
                })

                const existingTimer =
                    timers.get(
                        response.userId,
                    )

                if (existingTimer) {
                    clearTimeout(existingTimer)
                }

                const timer =
                    setTimeout(() => {
                        setTypingUsers(
                            (current) =>
                                current.filter(
                                    (user) =>
                                        user.userId !==
                                        response.userId,
                                ),
                        )

                        timers.delete(
                            response.userId,
                        )
                    }, 3000)

                timers.set(
                    response.userId,
                    timer,
                )

                return
            }

            const timer =
                timers.get(
                    response.userId,
                )

            if (timer) {
                clearTimeout(timer)

                timers.delete(
                    response.userId,
                )
            }

            setTypingUsers(
                (current) =>
                    current.filter(
                        (user) =>
                            user.userId !==
                            response.userId,
                    ),
            )
        }

        connection.on(
            'UserTyping',
            handleUserTyping,
        )

        return () => {
            connection.off(
                'UserTyping',
                handleUserTyping,
            )

            for (
                const timer
                of timers.values()
            ) {
                clearTimeout(timer)
            }

            timers.clear()

            setTypingUsers([])
        }
    }, [
        channelId,
        connection,
        currentUserId,
    ])

    const startTyping = useCallback(
        async () => {
            if (!channelId) {
                return
            }

            await connection.invoke(
                'TypingStarted',
                {
                    channelId,
                },
            )
        },
        [
            channelId,
            connection,
        ],
    )

    const stopTyping = useCallback(
        async () => {
            if (!channelId) {
                return
            }

            await connection.invoke(
                'TypingStopped',
                {
                    channelId,
                },
            )
        },
        [
            channelId,
            connection,
        ],
    )

    return {
        typingUsers,
        startTyping,
        stopTyping,
    }
}