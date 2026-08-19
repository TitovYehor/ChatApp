import {
    createContext,
    useContext,
    useMemo,
    useState,
    type ReactNode,
} from 'react'

import type { AuthenticatedUser } from '../../types/authTypes'

import {
    clearAuthentication,
    getAccessToken,
    getAuthenticatedUser,
    setAccessToken,
    setAuthenticatedUser,
} from '../../lib/authStorage'

interface AuthContextValue {
    user: AuthenticatedUser | null
    isAuthenticated: boolean

    login: (
        token: string,
        user: AuthenticatedUser,
    ) => void

    logout: () => void
}

const AuthContext = createContext<
    AuthContextValue | undefined
>(undefined)

interface AuthProviderProps {
    children: ReactNode
}

export function AuthProvider({
    children,
}: AuthProviderProps) {
    const [user, setUser] =
        useState<AuthenticatedUser | null>(
            getAuthenticatedUser(),
        )

    const [token, setToken] =
        useState<string | null>(
            getAccessToken(),
        )

    const value = useMemo<AuthContextValue>(
        () => ({
            user,

            isAuthenticated:
                token !== null && user !== null,

            login: (
                accessToken: string,
                authenticatedUser: AuthenticatedUser,
            ) => {
                setAccessToken(accessToken)
                setAuthenticatedUser(
                    authenticatedUser,
                )

                setToken(accessToken)
                setUser(authenticatedUser)
            },

            logout: () => {
                clearAuthentication()

                setToken(null)
                setUser(null)
            },
        }),
        [token, user],
    )

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    )
}

export function useAuth(): AuthContextValue {
    const context = useContext(AuthContext)

    if (!context) {
        throw new Error(
            'useAuth must be used within an AuthProvider',
        )
    }

    return context
}