import {
    apiRequest,
} from './client'

import type {
    UpdateUserProfileRequest,
    UserProfileResponse,
    PublicUserProfileResponse,
} from '../types/userTypes'

export function getProfile(
    userId: string,
) {
    return apiRequest<UserProfileResponse>(
        `/users/${userId}`,
    )
}

export function getPublicProfile(userId: string) {
    return apiRequest<PublicUserProfileResponse>(
        `/users/${userId}/public`,
    )
}

export function updateProfile(
    request: UpdateUserProfileRequest,
) {
    return apiRequest<UserProfileResponse>(
        '/users/me',
        {
            method: 'PUT',
            body: JSON.stringify(
                request,
            ),
        },
    )
}