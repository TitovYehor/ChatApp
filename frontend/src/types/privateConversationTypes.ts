export interface PrivateConversationResponse {
    id: string
    otherUserId: string
    otherUsername: string
    createdAt: string
}

export interface PrivateMessageResponse {
    id: string
    conversationId: string
    userId: string
    username: string
    content: string
    createdAt: string
    updatedAt: string | null
}

export interface CreatePrivateMessageRequest {
    content: string
}