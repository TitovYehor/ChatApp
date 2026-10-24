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

export interface PrivateUserTypingResponse {
    conversationId: string
    userId: string
    username: string
    isTyping: boolean
}