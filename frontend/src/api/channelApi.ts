import { apiRequest } from './client'

import type {
    ChannelResponse,
    CreateChannelRequest,
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