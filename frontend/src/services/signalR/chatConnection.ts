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

let stopRequested = false

const INITIAL_RETRY_DELAY = 2000
const MAX_RETRY_DELAY = 10000

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
        .configureLogging(
            LogLevel.Information,
        )
        .build()

    return connection
}

export async function startChatConnection(): Promise<void> {
    const chatConnection = getChatConnection()

    stopRequested = false

    if (chatConnection.state ===
        HubConnectionState.Connected
    ) {
        return
    }

    if (startPromise) {
        await startPromise
        return
    }

    startPromise =
        connectWithRetry(
            chatConnection,
        ).finally(() => {
            startPromise = null
        })

    await startPromise
}

async function connectWithRetry(
    chatConnection: HubConnection,
): Promise<void> {
    let retryDelay = INITIAL_RETRY_DELAY

    while (!stopRequested) {
        if (chatConnection.state ===
            HubConnectionState.Connected
        ) {
            return
        }

        try {
            console.info(
                'Attempting to connect to SignalR...',
            )

            await chatConnection.start()

            console.info(
                'SignalR connection established.',
            )

            return
        } catch (error) {
            if (stopRequested) {
                return
            }

            console.warn(
                `Failed to connect to SignalR.Retrying in ${ retryDelay } ms.`,
                error,
            )

            await delay(
                retryDelay,
            )

            retryDelay = Math.min(
                retryDelay * 2,
                MAX_RETRY_DELAY,
            )
        }
    }
}

function delay(
    milliseconds: number,
): Promise<void> {
    return new Promise(
        (resolve) => {
            setTimeout(
                resolve,
                milliseconds,
            )
        },
    )
}

export async function stopChatConnection(): Promise<void> {
    stopRequested = true

    if (startPromise) {
        await startPromise
    }

    if (!connection) {
        return
    }

    if (connection.state ===
        HubConnectionState.Disconnected
    ) {
        return
    }

    await connection.stop()
}

export async function joinChannel(
    channelId: string,
): Promise<void> {
    const chatConnection = getChatConnection()

    if (chatConnection.state !==
        HubConnectionState.Connected
    ) {
        throw new Error(
            'Chat connection is not connected',
        )
    }

    await chatConnection.invoke(
        'JoinChannel',
        {
            channelId,
        },
    )
}

export async function leaveChannel(
    channelId: string,
): Promise<void> {
    const chatConnection = getChatConnection()

    if (chatConnection.state !==
        HubConnectionState.Connected
    ) {
        return
    }

    await chatConnection.invoke(
        'LeaveChannel',
        {
            channelId,
        },
    )
}