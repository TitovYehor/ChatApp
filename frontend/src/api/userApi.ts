import {
    apiRequest,
} from './client'

import type {
    ChangePasswordRequest,
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

export function changePassword(
    request: ChangePasswordRequest,
) {
    return apiRequest<void>(
        '/users/me/password',
        {
            method: 'PUT',
            body: JSON.stringify(
                request,
            ),
        },
    )
}