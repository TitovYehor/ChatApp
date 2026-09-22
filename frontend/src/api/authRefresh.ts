import { API_BASE_URL } from './config'

import {
    clearAuthentication,
    setAccessToken,
    setAuthenticatedUser,
} from '../lib/authStorage'

import type {
    AuthResponse,
} from '../types/authTypes'

let refreshPromise:
    Promise<string> | null = null

async function performRefresh(): Promise<string> {
    const response = await fetch(
        `${API_BASE_URL}/auth/refresh`,
        {
            method: 'POST',
            credentials: 'include',
        },
    )

    if (!response.ok) {
        clearAuthentication()

        throw new Error('Unable to refresh authentication',
        )
    }

    const result = await response.json() as AuthResponse

    setAccessToken(
        result.accessToken,
    )

    setAuthenticatedUser(
        result.user,
    )

    return result.accessToken
}

export function refreshAccessToken():
    Promise<string> {
    if (refreshPromise) {
        return refreshPromise
    }

    refreshPromise = performRefresh()
        .finally(() => {
            refreshPromise = null
        })

    return refreshPromise
}