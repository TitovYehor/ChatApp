export interface UserPresenceChangedResponse {
    userId: string
    username: string
    isOnline: boolean
}

export interface OnlineUserResponse {
    userId: string
    username: string
}

export interface UserTypingResponse {
    userId: string
    username: string
    channelId: string
    isTyping: boolean
}