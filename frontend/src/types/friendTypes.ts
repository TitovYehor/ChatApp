export interface FriendResponse {
    userId: string
    username: string
    createdAt: string
}

export interface FriendSearchResponse {
    userId: string
    username: string
    isFriend: boolean
}