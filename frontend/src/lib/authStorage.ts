import type { AuthenticatedUser } from '../types/authTypes'

const ACCESS_TOKEN_KEY = 'chatapp_access_token'
const USER_KEY = 'chatapp_user'

export function getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY)
}

export function setAccessToken(token: string): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, token)
}

export function removeAccessToken(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY)
}

export function getAuthenticatedUser():
    | AuthenticatedUser
    | null {
    const value = localStorage.getItem(USER_KEY)

    if (!value) {
        return null
    }

    try {
        return JSON.parse(value) as AuthenticatedUser
    } catch {
        removeAuthenticatedUser()
        return null
    }
}

export function setAuthenticatedUser(
    user: AuthenticatedUser,
): void {
    localStorage.setItem(
        USER_KEY,
        JSON.stringify(user),
    )
}

export function removeAuthenticatedUser(): void {
    localStorage.removeItem(USER_KEY)
}

export function clearAuthentication(): void {
    removeAccessToken()
    removeAuthenticatedUser()
}