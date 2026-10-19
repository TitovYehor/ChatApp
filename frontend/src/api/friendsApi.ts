import { apiRequest } from './client'
import type {
    FriendResponse,
    FriendSearchResponse,
} from '../types/friendTypes'

export function getAll(): Promise<FriendResponse[]> {
    return apiRequest<FriendResponse[]>('/friends')
}

export function search(
    query: string,
): Promise<FriendSearchResponse[]> {
    const params = new URLSearchParams()

    if (query.trim()) {
        params.set('query', query.trim())
    }

    return apiRequest<FriendSearchResponse[]>(
        `/friends/search?${params.toString()}`,
    )
}

export function add(
    userId: string,
): Promise<void> {
    return apiRequest<void>(
        `/friends/${userId}`,
        {
            method: 'POST',
        },
    )
}

export function remove(
    userId: string,
): Promise<void> {
    return apiRequest<void>(
        `/friends/${userId}`,
        {
            method: 'DELETE',
        },
    )
}