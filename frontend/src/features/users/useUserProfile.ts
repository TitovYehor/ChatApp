import {
    useMutation,
    useQuery,
    useQueryClient,
} from '@tanstack/react-query'

import {
    changePassword,
} from '../../api/authApi'

import {
    getProfile,
    updateProfile,
} from '../../api/userApi'

import {
    useAuth,
} from '../auth/useAuth'

import type {
    AuthenticatedUser,
} from '../../types/authTypes'

import type {
    ChangePasswordRequest,
    UpdateUserProfileRequest
} from '../../types/userTypes'


const DEMO_ACCOUNT_EMAIL = 'demo@example.com'

const DEMO_ACCOUNT_RESTRICTION_MESSAGE = 'The demo account cannot change its username or password.'

export function useUserProfile() {
    const queryClient = useQueryClient()

    const {
        user,
        updateUser,
        updateAuthentication,
    } = useAuth()

    const isDemoAccount = user?.email?.toLowerCase() === DEMO_ACCOUNT_EMAIL

    const userId = user?.id ?? null

    const query = useQuery({
        queryKey: [
            'user-profile',
            userId,
        ],
        queryFn: () =>
            getProfile(userId!),
        enabled: userId !== null,
    })

    const updateProfileMutation = useMutation({
        mutationFn: (request: UpdateUserProfileRequest) => {
            if (isDemoAccount) {
                throw new Error(DEMO_ACCOUNT_RESTRICTION_MESSAGE)
            }

            return updateProfile(request)
        },

        onSuccess: (profile) => {
            queryClient.setQueryData(
                ['user-profile', userId],
                profile,
            )

            const authenticatedUser: AuthenticatedUser = {
                id: profile.id,
                username: profile.username,
                email: profile.email,
            }

            updateUser(authenticatedUser)
        },
    })

    const changePasswordMutation = useMutation({
        mutationFn: (request: ChangePasswordRequest) => {
            if (isDemoAccount) {
                throw new Error(DEMO_ACCOUNT_RESTRICTION_MESSAGE)
            }

            return changePassword(request)
        },

        onSuccess: (response) => {
            updateAuthentication(
                response.accessToken,
                response.user,
            )
        },
    })

    return {
        profile: query.data ?? null,

        isLoading: query.isLoading,

        error: query.error
            ? 'Failed to load profile'
            : null,

        reload: query.refetch,

        isDemoAccount,

        updateProfile: updateProfileMutation.mutateAsync,

        isUpdatingProfile: updateProfileMutation.isPending,

        updateProfileError: updateProfileMutation.error
            ? updateProfileMutation.error instanceof Error
                ? updateProfileMutation.error.message
                : 'Failed to update profile'
            : null,

        changePassword: changePasswordMutation.mutateAsync,

        isChangingPassword: changePasswordMutation.isPending,

        changePasswordError: changePasswordMutation.error
            ? changePasswordMutation.error instanceof Error
                ? changePasswordMutation.error.message
                : 'Failed to change password'
            : null,
    }
}