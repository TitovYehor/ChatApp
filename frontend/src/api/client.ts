import { API_BASE_URL } from './config'
import {
    getAccessToken,
} from '../lib/authStorage'
import { ApiError } from './ApiError'
import {
    refreshAccessToken,
} from './authRefresh'

export async function apiRequest<T>(
    endpoint: string,
    options: RequestInit = {},
): Promise<T> {
    const response = await sendRequest(
        endpoint,
        options,
    )

    if (response.status === 401 &&
        endpoint !== '/auth/refresh'
    ) {
        await refreshAccessToken()

        const retryResponse = await sendRequest(
            endpoint,
            options,
        )

        const retryBody = await readResponseBody(
            retryResponse,
        )

        if (!retryResponse.ok) {
            throw new ApiError(
                retryResponse.status,
                `API request failed: ${retryResponse.status}`,
                retryBody,
            )
        }

        return retryBody as T
    }

    const responseBody = await readResponseBody(
        response,
    )

    if (!response.ok) {
        throw new ApiError(
            response.status,
            `API request failed: ${response.status}`,
            responseBody,
        )
    }

    return responseBody as T
}

async function sendRequest(
    endpoint: string,
    options: RequestInit,
): Promise<Response> {
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

    return fetch(
        `${API_BASE_URL}${endpoint}`,
        {
            ...options,
            headers,
            credentials: 'include',
        },
    )
}

async function readResponseBody(
    response: Response,
): Promise<unknown> {
    if (response.status === 204) {
        return undefined
    }

    const contentType = response.headers.get(
        'content-type',
    )

    if (contentType?.includes(
            'application/json',
        )
    ) {
        return response.json()
    }

    return response.text()
}