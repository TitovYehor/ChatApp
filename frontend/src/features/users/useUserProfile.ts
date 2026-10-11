
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
} from '../../types/userTypes'

export function useUserProfile() {
    const queryClient = useQueryClient()

    const {
        user,
        updateUser,
        updateAuthentication,
    } = useAuth()

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
        mutationFn: updateProfile,

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
        mutationFn: (request: ChangePasswordRequest) =>
            changePassword(request),

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

        updateProfile: updateProfileMutation.mutateAsync,

        isUpdatingProfile: updateProfileMutation.isPending,

        updateProfileError: updateProfileMutation.error
            ? 'Failed to update profile'
            : null,

        changePassword: changePasswordMutation.mutateAsync,

        isChangingPassword: changePasswordMutation.isPending,

        changePasswordError: changePasswordMutation.error
            ? 'Failed to change password'
            : null,
    }
}