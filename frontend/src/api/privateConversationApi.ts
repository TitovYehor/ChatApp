import { apiRequest } from './client'

import type {
    PagedResult,
} from '../types/pagedResultType'

import type {
    CreatePrivateMessageRequest,
    PrivateConversationResponse,
    PrivateMessageResponse,
} from '../types/privateConversationTypes'

export function getAll(): Promise<PrivateConversationResponse[]> {
    return apiRequest<PrivateConversationResponse[]>(
        '/conversations',
    )
}

export function getOrCreate(
    otherUserId: string,
): Promise<PrivateConversationResponse> {
    return apiRequest<PrivateConversationResponse>(
        `/conversations/${encodeURIComponent(otherUserId)}`,
        {
            method: 'POST',
        },
    )
}

export function getMessages(
    conversationId: string,
    pageNumber = 1,
    pageSize = 50,
): Promise<PagedResult<PrivateMessageResponse>> {
    const params = new URLSearchParams({
        pageNumber: pageNumber.toString(),
        pageSize: pageSize.toString(),
    })

    return apiRequest<PagedResult<PrivateMessageResponse>>(
        `/conversations/${encodeURIComponent(conversationId)}/messages?${params.toString()}`,
    )
}

export function sendMessage(
    conversationId: string,
    request: CreatePrivateMessageRequest,
): Promise<PrivateMessageResponse> {
    return apiRequest<PrivateMessageResponse>(
        `/conversations/${encodeURIComponent(conversationId)}/messages`,
        {
            method: 'POST',
            body: JSON.stringify(request),
        },
    )
}

export function updateMessage(
    conversationId: string,
    messageId: string,
    request: { content: string },
): Promise<PrivateMessageResponse> {
    return apiRequest<PrivateMessageResponse>(
        `/conversations/${encodeURIComponent(conversationId)}/messages/${encodeURIComponent(messageId)}`,
        {
            method: 'PUT',
            body: JSON.stringify(request),
        },
    )
}

export function deleteMessage(
    conversationId: string,
    messageId: string,
): Promise<void> {
    return apiRequest<void>(
        `/conversations/${encodeURIComponent(conversationId)}/messages/${encodeURIComponent(messageId)}`,
        {
            method: 'DELETE',
        },
    )
}