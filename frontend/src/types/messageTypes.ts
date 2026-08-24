export interface CreateMessageRequest {
    content: string
}

export interface UpdateMessageRequest {
    content: string
}

export interface MessageQuery {
    pageNumber?: number
    pageSize?: number
    search?: string
}

export interface MessageResponse {
    id: string
    channelId: string
    userId: string
    username: string
    content: string
    createdAt: string
    updatedAt: string | null
}

export interface MessageDeletedResponse {
    messageId: string
    channelId: string
}