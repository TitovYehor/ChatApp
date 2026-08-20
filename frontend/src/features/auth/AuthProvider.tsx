import {
    useMemo,
    useState,
    type ReactNode,
} from 'react'

import {
    clearAuthentication,
    getAccessToken,
    getAuthenticatedUser,
    setAccessToken,
    setAuthenticatedUser,
} from '../../lib/authStorage'

import {
    AuthContext,
    type AuthContextValue,
} from './AuthContext'

interface AuthProviderProps {
    children: ReactNode
}

export function AuthProvider({
    children,
}: AuthProviderProps) {
    const [user, setUser] =
        useState(
            getAuthenticatedUser(),
        )

    const [token, setToken] =
        useState(
            getAccessToken(),
        )

    const value = useMemo<AuthContextValue>(
        () => ({
            user,

            isAuthenticated:
                token !== null &&
                user !== null,

            login: (
                accessToken: string,
                authenticatedUser,
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