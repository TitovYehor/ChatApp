import {
    useEffect,
} from 'react'

import {
    getChatConnection,
    joinChannel,
    leaveChannel,
} from '../../services/signalR/chatConnection'

export function useChannelSignalR(
    channelId: string | null,
) {
    useEffect(() => {
        if (!channelId) {
            return
        }

        let isActive = true

        async function join() {
            try {
                await joinChannel(
                    channelId ?? "",
                )
            } catch (error) {
                if (!isActive) {
                    return
                }

                console.error(
                    'Failed to join channel',
                    error,
                )
            }
        }

        void join()

        return () => {
            isActive = false

            void leaveChannel(
                channelId,
            )
        }
    }, [channelId])

    return getChatConnection()
}