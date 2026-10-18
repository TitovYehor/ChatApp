import { apiRequest } from './client'

import type {
    NotificationResponse,
} from '../types/notificationTypes'

export function getLatest(
    limit = 50,
): Promise<NotificationResponse[]> {
    const params = new URLSearchParams({
        limit: String(limit),
    })

    return apiRequest<NotificationResponse[]>(
        `/notifications?${params.toString()}`,
    )
}

export function markAsRead(
    notificationId: string,
): Promise<void> {
    return apiRequest<void>(
        `/notifications/${notificationId}/read`,
        {
            method: 'PUT',
        },
    )
}

export interface MarkAllAsReadResponse {
    updatedCount: number
}

export function markAllAsRead():
    Promise<MarkAllAsReadResponse> {
    return apiRequest<MarkAllAsReadResponse>(
        '/notifications/read-all',
        {
            method: 'PUT',
        },
    )
}