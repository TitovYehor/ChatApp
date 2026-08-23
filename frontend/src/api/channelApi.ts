import { apiRequest } from './client'

import type {
    ChannelResponse,
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