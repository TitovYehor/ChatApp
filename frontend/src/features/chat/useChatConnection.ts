import {
    useEffect,
    useState,
} from 'react'

import {
    getChatConnection,
    startChatConnection,
    stopChatConnection,
} from '../../services/signalR/chatConnection'

export function useChatConnection() {
    const [isConnected, setIsConnected] =
        useState(false)

    const [error, setError] =
        useState<string | null>(null)

    useEffect(() => {
        let isMounted = true

        async function connect() {
            try {
                await startChatConnection()

                if (isMounted) {
                    setIsConnected(true)
                    setError(null)
                }
            } catch {
                if (isMounted) {
                    setError(
                        'Failed to connect to chat',
                    )
                }
            }
        }

        void connect()

        return () => {
            isMounted = false

            void stopChatConnection()
        }
    }, [])

    return {
        connection: getChatConnection(),
        isConnected,
        error,
    }
}