import { API_BASE_URL } from './config'
import { getAccessToken } from '../lib/authStorage'
import { ApiError } from './ApiError'

export async function apiRequest<T>(
    endpoint: string,
    options: RequestInit = {},
): Promise<T> {
    const token = getAccessToken()

    const headers = new Headers(options.headers)

    headers.set('Content-Type', 'application/json')

    if (token) {
        headers.set(
            'Authorization',
            `Bearer ${token}`,
        )
    }

    const response = await fetch(
        `${API_BASE_URL}${endpoint}`,
        {
            ...options,
            headers,
        },
    )

    if (!response.ok) {
        throw new ApiError(
            response.status,
            `API request failed: ${response.status}`,
        )
    }

    return response.json() as Promise<T>
}