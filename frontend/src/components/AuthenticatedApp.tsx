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
        if (isAuthenticated) {
            void startChatConnection()
            return
        }

        void stopChatConnection()
    }, [isAuthenticated])

    return children
}

export default AuthenticatedApp