import { API_BASE_URL } from './config'
import { getAccessToken } from '../lib/authStorage'
import { ApiError } from './ApiError'

export async function apiRequest<T>(
    endpoint: string,
    options: RequestInit = {},
): Promise<T> {
    const token = getAccessToken()

    const headers = new Headers(options.headers)

    if (options.body) {
        headers.set(
            'Content-Type',
            'application/json',
        )
    }

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

    const responseBody = await readResponseBody(response)

    if (!response.ok) {
        throw new ApiError(
            response.status,
            `API request failed: ${response.status}`,
            responseBody,
        )
    }

    return responseBody as T
}

async function readResponseBody(
    response: Response,
): Promise<unknown> {
    if (response.status === 204) {
        return undefined
    }

    const contentType = response.headers.get('content-type')

    if (contentType?.includes('application/json')) {
        return response.json()
    }

    return response.text()
}