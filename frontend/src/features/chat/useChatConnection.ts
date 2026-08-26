import {
    useEffect,
    useState,
} from 'react'

import {
    HubConnectionState,
} from '@microsoft/signalr'

import {
    getChatConnection,
    scheduleStopChatConnection,
    startChatConnection,
} from '../../services/signalR/chatConnection'

export function useChatConnection() {
    const connection =
        getChatConnection()

    const [
        connectionState,
        setConnectionState,
    ] = useState<HubConnectionState>(
        connection.state,
    )

    const [error, setError] =
        useState<string | null>(null)

    useEffect(() => {
        const handleReconnecting = () => {
            setConnectionState(
                HubConnectionState.Reconnecting,
            )
        }

        const handleReconnected = () => {
            setConnectionState(
                HubConnectionState.Connected,
            )

            setError(null)
        }

        const handleClosed = () => {
            setConnectionState(
                HubConnectionState.Disconnected,
            )
        }

        connection.onreconnecting(
            handleReconnecting,
        )

        connection.onreconnected(
            handleReconnected,
        )

        connection.onclose(
            handleClosed,
        )

        async function connect() {
            try {
                await startChatConnection()

                setConnectionState(
                    connection.state,
                )

                setError(null)
            } catch {
                setConnectionState(
                    HubConnectionState.Disconnected,
                )

                setError(
                    'Failed to connect to chat',
                )
            }
        }

        void connect()

        return () => {
            connection.off(
                'reconnecting',
                handleReconnecting,
            )

            connection.off(
                'reconnected',
                handleReconnected,
            )

            connection.off(
                'close',
                handleClosed,
            )

            scheduleStopChatConnection()
        }
    }, [connection])

    return {
        connection,
        isConnected:
            connectionState ===
            HubConnectionState.Connected,
        connectionState,
        error,
    }
}