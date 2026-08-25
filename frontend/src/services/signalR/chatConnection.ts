import {
    HubConnection,
    HubConnectionBuilder,
    HubConnectionState,
    LogLevel,
} from '@microsoft/signalr'

import { SIGNALR_HUB_URL } from '../../api/config'
import { getAccessToken } from '../../lib/authStorage'

let connection: HubConnection | null = null

export function getChatConnection(): HubConnection {
    if (connection) {
        return connection
    }

    connection = new HubConnectionBuilder()
        .withUrl(
            SIGNALR_HUB_URL,
            {
                accessTokenFactory:
                    () =>
                        getAccessToken() ?? '',
            },
        )
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build()

    return connection
}

export async function startChatConnection(): Promise<void> {
    const chatConnection =
        getChatConnection()

    if (
        chatConnection.state ===
        HubConnectionState.Connected
    ) {
        return
    }

    if (
        chatConnection.state ===
        HubConnectionState.Connecting
    ) {
        return
    }

    await chatConnection.start()
}

export async function stopChatConnection(): Promise<void> {
    if (!connection) {
        return
    }

    if (
        connection.state ===
        HubConnectionState.Disconnected
    ) {
        return
    }

    await connection.stop()
}