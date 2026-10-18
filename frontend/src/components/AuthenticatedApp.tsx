import { useEffect } from 'react'

import { useAuth } from '../features/auth/useAuth'
import NotificationRealtimeListener from '../features/notifications/NotificationRealtimeListener'

import {
    startChatConnection,
    stopChatConnection,
} from '../services/signalR/chatConnection'

interface AuthenticatedAppProps {
    children: React.ReactNode
}

function AuthenticatedApp({ children }: AuthenticatedAppProps) {
    const { isAuthenticated } = useAuth()

    useEffect(() => {
        if (isAuthenticated) {
            void startChatConnection()
            return
        }

        void stopChatConnection()
    }, [isAuthenticated])

    if (!isAuthenticated) {
        return <>{children}</>
    }

    return (
        <>
            <NotificationRealtimeListener />
            {children}
        </>
    )
}

export default AuthenticatedApp