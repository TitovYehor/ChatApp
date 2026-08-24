import { apiRequest } from './client'

import type {
    CreateMessageRequest,
    MessageQuery,
    MessageResponse,
    UpdateMessageRequest,
} from '../types/messageTypes'

import type {
    PagedResult,
} from '../types/pagedResultType'

export function getByChannelId(
    channelId: string,
    query: MessageQuery = {},
): Promise<PagedResult<MessageResponse>> {
    const params = new URLSearchParams()

    if (query.pageNumber !== undefined) {
        params.set(
            'PageNumber',
            query.pageNumber.toString(),
        )
    }

    if (query.pageSize !== undefined) {
        params.set(
            'PageSize',
            query.pageSize.toString(),
        )
    }

    if (query.search) {
        params.set('Search', query.search)
    }

    const queryString = params.toString()

    return apiRequest<PagedResult<MessageResponse>>(
        `/channels/${channelId}/messages${queryString
            ? `?${queryString}`
            : ''
        }`,
    )
}

export function getById(
    messageId: string,
): Promise<MessageResponse> {
    return apiRequest<MessageResponse>(
        `/messages/${messageId}`,
    )
}

export function create(
    channelId: string,
    request: CreateMessageRequest,
): Promise<MessageResponse> {
    return apiRequest<MessageResponse>(
        `/channels/${channelId}/messages`,
        {
            method: 'POST',
            body: JSON.stringify(request),
        },
    )
}

export function update(
    messageId: string,
    request: UpdateMessageRequest,
): Promise<MessageResponse> {
    return apiRequest<MessageResponse>(
        `/messages/${messageId}`,
        {
            method: 'PUT',
            body: JSON.stringify(request),
        },
    )
}

export function remove(
    messageId: string,
): Promise<void> {
    return apiRequest<void>(
        `/messages/${messageId}`,
        {
            method: 'DELETE',
        },
    )
}