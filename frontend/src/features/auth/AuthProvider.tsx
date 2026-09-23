import {
    useEffect,
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

import {
    refreshAccessToken,
} from '../../api/authRefresh'

import {
    logout as logoutApi,
} from '../../api/authApi'

interface AuthProviderProps {
    children: ReactNode
}

export function AuthProvider({
    children,
}: AuthProviderProps) {
    const [user, setUser] = useState(
        getAuthenticatedUser(),
    )

    const [token, setToken] = useState(
        getAccessToken(),
    )

    const [isInitializing, setIsInitializing] =
        useState(true)

    useEffect(() => {
        let mounted = true

        async function initializeAuthentication() {
            const existingToken = getAccessToken()

            const existingUser = getAuthenticatedUser()

            if (!existingToken ||
                !existingUser
            ) {
                if (mounted) {
                    setIsInitializing(false)
                }

                return
            }

            try {
                const newToken = await refreshAccessToken()

                if (!mounted) {
                    return
                }

                setToken(newToken)

                setUser(
                    getAuthenticatedUser(),
                )
            } catch {
                if (!mounted) {
                    return
                }

                clearAuthentication()

                setToken(null)
                setUser(null)
            } finally {
                if (mounted) {
                    setIsInitializing(false)
                }
            }
        }

        void initializeAuthentication()

        return () => {
            mounted = false
        }
    }, [])

    const value =
        useMemo<AuthContextValue>(
            () => ({
                user,

                isAuthenticated:
                    token !== null &&
                    user !== null,

                isInitializing,

                login: (
                    accessToken,
                    authenticatedUser,
                ) => {
                    setAccessToken(
                        accessToken,
                    )

                    setAuthenticatedUser(
                        authenticatedUser,
                    )

                    setToken(
                        accessToken,
                    )

                    setUser(
                        authenticatedUser,
                    )
                },

                logout: async () => {
                    try {
                        await logoutApi()
                    } finally {
                        clearAuthentication()

                        setToken(null)
                        setUser(null)
                    }
                },
            }),
            [
                token,
                user,
                isInitializing,
            ],
        )

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    )
}