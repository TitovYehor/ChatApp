import {
    apiRequest,
} from './client'

import type {
    UpdateUserProfileRequest,
    UserProfileResponse,
} from '../types/userTypes'

export function getProfile(
    userId: string,
) {
    return apiRequest<UserProfileResponse>(
        `/users/${userId}`,
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