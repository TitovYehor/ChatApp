import { apiRequest } from './client'

import type {
    AuthResponse,
    LoginRequest,
    RegisterRequest,
} from '../types/authTypes'

import type {
    ChangePasswordRequest,
} from '../types/userTypes'

export function register(
    request: RegisterRequest,
): Promise<AuthResponse> {
    return apiRequest<AuthResponse>(
        '/auth/register',
        {
            method: 'POST',
            body: JSON.stringify(request),
            credentials: 'include',
        },
    )
}

export function login(
    request: LoginRequest,
): Promise<AuthResponse> {
    return apiRequest<AuthResponse>(
        '/auth/login',
        {
            method: 'POST',
            body: JSON.stringify(request),
            credentials: 'include',
        },
    )
}

export function refresh(): Promise<AuthResponse> {
    return apiRequest<AuthResponse>(
        '/auth/refresh',
        {
            method: 'POST',
            credentials: 'include',
        },
    )
}

export function logout(): Promise<void> {
    return apiRequest<void>(
        '/auth/logout',
        {
            method: 'POST',
            credentials: 'include',
        },
    )
}

export function changePassword(
    request: ChangePasswordRequest,
): Promise<AuthResponse> {
    return apiRequest<AuthResponse>(
        '/auth/change-password',
        {
            method: 'POST',
            body: JSON.stringify(request),
            credentials: 'include',
        },
    )
}