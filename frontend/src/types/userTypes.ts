export interface UserProfileResponse {
    id: string
    username: string
    email: string
    createdAt: string
}

export interface UpdateUserProfileRequest {
    username: string
}

export interface ChangePasswordRequest {
    currentPassword: string
    newPassword: string
}