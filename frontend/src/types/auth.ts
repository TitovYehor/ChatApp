export interface RegisterRequest {
    username: string
    email: string
    password: string
}

export interface LoginRequest {
    email: string
    password: string
}

export interface AuthenticatedUser {
    id: string
    username: string
    email: string
}

export interface AuthResponse {
    accessToken: string
    user: AuthenticatedUser
}