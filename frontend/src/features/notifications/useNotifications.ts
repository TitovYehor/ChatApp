import {
    useMutation,
    useQuery,
    useQueryClient,
} from '@tanstack/react-query'

import {
    getLatest,
    markAllAsRead,
    markAsRead,
} from '../../api/notificationsApi'

import type { NotificationResponse } from '../../types/notificationTypes'

export const notificationKeys = {
    all: ['notifications'] as const,
    latest: (limit: number) =>
        [...notificationKeys.all, 'latest', limit] as const,
}

const NOTIFICATION_LIMIT = 50

export function useNotifications() {
    const queryClient = useQueryClient()
    const queryKey = notificationKeys.latest(NOTIFICATION_LIMIT)

    const notificationsQuery = useQuery({
        queryKey,
        queryFn: () => getLatest(NOTIFICATION_LIMIT),
    })

    const markReadMutation = useMutation({
        mutationFn: markAsRead,

        onMutate: async (notificationId) => {
            await queryClient.cancelQueries({ queryKey })

            const previousNotifications =
                queryClient.getQueryData<NotificationResponse[]>(queryKey)

            queryClient.setQueryData<NotificationResponse[]>(
                queryKey,
                (current) =>
                    current?.map((notification) =>
                        notification.id === notificationId
                            ? { ...notification, isRead: true }
                            : notification
                    )
            )

            return { previousNotifications }
        },

        onError: (_error, _notificationId, context) => {
            if (context?.previousNotifications) {
                queryClient.setQueryData(
                    queryKey,
                    context.previousNotifications
                )
            }
        },

        onSettled: () => {
            void queryClient.invalidateQueries({ queryKey })
        },
    })

    const markAllReadMutation = useMutation({
        mutationFn: markAllAsRead,

        onMutate: async () => {
            await queryClient.cancelQueries({ queryKey })

            const previousNotifications =
                queryClient.getQueryData<NotificationResponse[]>(queryKey)

            queryClient.setQueryData<NotificationResponse[]>(
                queryKey,
                (current) =>
                    current?.map((notification) => ({
                        ...notification,
                        isRead: true,
                    }))
            )

            return { previousNotifications }
        },

        onError: (_error, _variables, context) => {
            if (context?.previousNotifications) {
                queryClient.setQueryData(
                    queryKey,
                    context.previousNotifications
                )
            }
        },

        onSettled: () => {
            void queryClient.invalidateQueries({ queryKey })
        },
    })

    return {
        notifications: notificationsQuery.data ?? [],
        isLoading: notificationsQuery.isLoading,
        isError: notificationsQuery.isError,
        markAsRead: markReadMutation.mutate,
        markAllAsRead: markAllReadMutation.mutate,
        isMarkingAllAsRead: markAllReadMutation.isPending,
    }
}