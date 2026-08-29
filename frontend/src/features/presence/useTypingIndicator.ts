import {
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

    const [typingUsers, setTypingUsers] =
        useState<UserTypingResponse[]>([])

    const stopTimers =
        useRef<
            Map<string, ReturnType<typeof setTimeout>>
        >(new Map())

    useEffect(() => {
        if (!channelId) {
            setTypingUsers([])
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
                    stopTimers.current.get(
                        response.userId,
                    )

                if (existingTimer) {
                    clearTimeout(
                        existingTimer,
                    )
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

                        stopTimers.current.delete(
                            response.userId,
                        )
                    }, 3000)

                stopTimers.current.set(
                    response.userId,
                    timer,
                )

                return
            }

            const timer =
                stopTimers.current.get(
                    response.userId,
                )

            if (timer) {
                clearTimeout(timer)

                stopTimers.current.delete(
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
                of stopTimers.current.values()
            ) {
                clearTimeout(timer)
            }

            stopTimers.current.clear()

            setTypingUsers([])
        }
    }, [
        channelId,
        connection,
        currentUserId,
    ])

    const startTyping = async () => {
        if (!channelId) {
            return
        }

        await connection.invoke(
            'TypingStarted',
            {
                channelId,
            },
        )
    }

    const stopTyping = async () => {
        if (!channelId) {
            return
        }

        await connection.invoke(
            'TypingStopped',
            {
                channelId,
            },
        )
    }

    return {
        typingUsers,
        startTyping,
        stopTyping,
    }
}