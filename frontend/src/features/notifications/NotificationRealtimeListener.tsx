import { useEffect } from 'react'
import { useQueryClient } from '@tanstack/react-query'

import { getChatConnection } from '../../services/signalR/chatConnection'
import { SignalREvents } from '../../types/signalREvents'
import type { NotificationResponse } from '../../types/notificationTypes'
import { notificationKeys } from './useNotifications'

const NOTIFICATION_LIMIT = 50

export default function NotificationRealtimeListener() {
    const queryClient = useQueryClient()

    useEffect(() => {
        const connection = getChatConnection()
        const queryKey = notificationKeys.latest(NOTIFICATION_LIMIT)

        const handleNotificationCreated = (
            notification: NotificationResponse
        ) => {
            queryClient.setQueryData<NotificationResponse[]>(
                queryKey,
                (current) => {
                    if (!current) {
                        return current
                    }

                    const withoutDuplicate = current.filter(
                        (item) => item.id !== notification.id
                    )

                    return [notification, ...withoutDuplicate].slice(
                        0,
                        NOTIFICATION_LIMIT
                    )
                }
            )

            void queryClient.invalidateQueries({ queryKey })
        }

        connection.on(
            SignalREvents.NotificationCreated,
            handleNotificationCreated
        )

        return () => {
            connection.off(
                SignalREvents.NotificationCreated,
                handleNotificationCreated
            )
        }
    }, [queryClient])

    return null
}