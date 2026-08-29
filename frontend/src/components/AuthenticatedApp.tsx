import {
    useEffect,
} from 'react'

import {
    useAuth,
} from '../features/auth/useAuth'

import {
    startChatConnection,
    stopChatConnection,
} from '../services/signalR/chatConnection'

interface AuthenticatedAppProps {
    children: React.ReactNode
}

function AuthenticatedApp({
    children,
}: AuthenticatedAppProps) {
    const {
        isAuthenticated,
    } = useAuth()

    useEffect(() => {
        if (!isAuthenticated) {
            void stopChatConnection()
            return
        }

        let cancelled = false

        async function connect() {
            try {
                await startChatConnection()
            } catch (error) {
                if (cancelled) {
                    return
                }

                console.error(
                    'Failed to start chat connection',
                    error,
                )
            }
        }

        void connect()

        return () => {
            cancelled = true
            void stopChatConnection()
        }
    }, [isAuthenticated])

    return children
}

export default AuthenticatedApp