import {
    useEffect,
} from 'react'

import {
    useAuth,
} from '../features/auth/useAuth'

import {
    startChatConnection,
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
            return
        }

        void startChatConnection()
    }, [isAuthenticated])

    return children
}

export default AuthenticatedApp