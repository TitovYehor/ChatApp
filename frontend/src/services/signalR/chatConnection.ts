import {
    HubConnection,
    HubConnectionBuilder,
    HubConnectionState,
    LogLevel,
} from '@microsoft/signalr'

import { SIGNALR_HUB_URL } from '../../api/config'
import { getAccessToken } from '../../lib/authStorage'

let connection: HubConnection | null = null

let startPromise: Promise<void> | null = null

let stopTimer: ReturnType<typeof setTimeout> | null = null

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

    if (stopTimer !== null) {
        clearTimeout(stopTimer)
        stopTimer = null
    }

    if (
        chatConnection.state ===
        HubConnectionState.Connected
    ) {
        return
    }

    if (startPromise) {
        await startPromise
        return
    }

    startPromise = chatConnection
        .start()
        .finally(() => {
            startPromise = null
        })

    await startPromise
}

export function scheduleStopChatConnection(): void {
    if (stopTimer !== null) {
        clearTimeout(stopTimer)
    }

    stopTimer = setTimeout(() => {
        stopTimer = null

        void stopChatConnection()
    }, 0)
}

export async function stopChatConnection(): Promise<void> {
    if (!connection) {
        return
    }

    if (stopTimer !== null) {
        clearTimeout(stopTimer)
        stopTimer = null
    }

    if (startPromise) {
        await startPromise
    }

    if (
        connection.state ===
        HubConnectionState.Disconnected
    ) {
        return
    }

    await connection.stop()
}