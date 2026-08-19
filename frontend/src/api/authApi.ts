import { apiRequest } from './client'
import type {
    AuthResponse,
    LoginRequest,
    RegisterRequest,
} from '../types/authTypes'

export function register(
    request: RegisterRequest,
): Promise<AuthResponse> {
    return apiRequest<AuthResponse>(
        '/auth/register',
        {
            method: 'POST',
            body: JSON.stringify(request),
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
        },
    )
}