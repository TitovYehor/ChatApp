import { createContext } from 'react'

import type { AuthenticatedUser } from '../../types/authTypes'

export interface AuthContextValue {
    user: AuthenticatedUser | null
    isAuthenticated: boolean

    login: (
        token: string,
        user: AuthenticatedUser,
    ) => void

    logout: () => void
}

export const AuthContext =
    createContext<AuthContextValue | undefined>(
        undefined,
    )