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
    PrivateUserTypingResponse,
} from '../../types/presenceTypes'

import {
    SignalREvents
} from '../../types/signalREvents'

import {
    SignalRMethods
} from '../../types/signalRMethods'

const TYPING_EXPIRATION_MS = 3000

interface PrivateTypingState {
    conversationId: string | null
    users: PrivateUserTypingResponse[]
}

export function usePrivateTypingIndicator(
    conversationId: string | null,
    currentUserId: string | null,
) {
    const connection = getChatConnection()

    const [typingState, setTypingState] = useState<PrivateTypingState>({
        conversationId: null,
        users: [],
    })

    const timers = useRef<
        Map<string, ReturnType<typeof setTimeout>>
    >(new Map())

    useEffect(() => {
        if (!conversationId) {
            return
        }

        const activeConversationId = conversationId

        const handlePrivateUserTyping = (
            response: PrivateUserTypingResponse,
        ) => {
            if (
                response.conversationId !== activeConversationId ||
                response.userId === currentUserId
            ) {
                return
            }

            const existingTimer = timers.current.get(response.userId)

            if (existingTimer) {
                clearTimeout(existingTimer)
                timers.current.delete(response.userId)
            }

            if (!response.isTyping) {
                setTypingState((current) => {
                    const users =
                        current.conversationId === activeConversationId
                            ? current.users
                            : []

                    return {
                        conversationId: activeConversationId,
                        users: users.filter(
                            (user) => user.userId !== response.userId,
                        ),
                    }
                })

                return
            }

            setTypingState((current) => {
                const users =
                    current.conversationId === activeConversationId
                        ? current.users
                        : []

                const exists = users.some(
                    (user) => user.userId === response.userId,
                )

                return {
                    conversationId: activeConversationId,
                    users: exists
                        ? users.map((user) =>
                            user.userId === response.userId
                                ? response
                                : user,
                        )
                        : [...users, response],
                }
            })

            const timer = setTimeout(() => {
                setTypingState((current) => ({
                    conversationId: activeConversationId,
                    users:
                        current.conversationId === activeConversationId
                            ? current.users.filter(
                                (user) => user.userId !== response.userId,
                            )
                            : current.users,
                }))

                timers.current.delete(response.userId)
            }, TYPING_EXPIRATION_MS)

            timers.current.set(response.userId, timer)
        }

        connection.on(
            SignalREvents.PrivateUserTyping,
            handlePrivateUserTyping,
        )

        return () => {
            connection.off(
                SignalREvents.PrivateUserTyping,
                handlePrivateUserTyping,
            )

            for (const timer of timers.current.values()) {
                clearTimeout(timer)
            }

            timers.current.clear()
        }
    }, [connection, conversationId, currentUserId])

    const startTyping = useCallback(async () => {
        if (!conversationId) {
            return
        }

        await connection.invoke(
            SignalRMethods.PrivateTypingStarted,
            conversationId,
        )
    }, [connection, conversationId])

    const stopTyping = useCallback(async () => {
        if (!conversationId) {
            return
        }

        await connection.invoke(
            SignalRMethods.PrivateTypingStopped,
            conversationId,
        )
    }, [connection, conversationId])

    const typingUsers =
        typingState.conversationId === conversationId
            ? typingState.users
            : []

    return {
        typingUsers,
        startTyping,
        stopTyping,
    }
}