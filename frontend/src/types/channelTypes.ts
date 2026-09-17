export interface CreateChannelRequest {
    name: string
}

export interface UpdateChannelRequest {
    name: string
}

export interface ChannelResponse {
    id: string
    workspaceId: string
    name: string
    type: number
    createdAt: string
}

export interface ChannelDeletedResponse {
    channelId: string
    workspaceId: string
}