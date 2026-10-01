import { apiRequest } from './client'

import type {
    ChannelResponse,
    CreateChannelRequest,
    UpdateChannelRequest,
    ChannelSearchRequest,
} from '../types/channelTypes'

export function getByWorkspaceId(
    workspaceId: string,
): Promise<ChannelResponse[]> {
    return apiRequest<ChannelResponse[]>(
        `/workspaces/${workspaceId}/channels`,
    )
}

export function getById(
    channelId: string,
): Promise<ChannelResponse> {
    return apiRequest<ChannelResponse>(
        `/channels/${channelId}`,
    )
}

export function search(
    workspaceId: string,
    request: ChannelSearchRequest,
): Promise<ChannelResponse[]> {
    const params = new URLSearchParams()

    if (request.query) {
        params.set('query', request.query)
    }

    return apiRequest<ChannelResponse[]>(
        `/workspaces/${workspaceId}/channels/search?${params.toString()}`,
    )
}

export function create(
    workspaceId: string,
    request: CreateChannelRequest,
): Promise<ChannelResponse> {
    return apiRequest<ChannelResponse>(
        `/workspaces/${workspaceId}/channels`,
        {
            method: 'POST',
            body: JSON.stringify(request),
        },
    )
}

export function update(
    channelId: string,
    request: UpdateChannelRequest,
): Promise<ChannelResponse> {
    return apiRequest<ChannelResponse>(
        `/channels/${channelId}`,
        {
            method: 'PUT',
            body: JSON.stringify(request),
        },
    )
}

export function remove(
    channelId: string,
): Promise<void> {
    return apiRequest<void>(
        `/channels/${channelId}`,
        {
            method: 'DELETE',
        },
    )
}