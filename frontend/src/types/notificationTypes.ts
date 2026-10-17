export interface NotificationResponse {
    id: string
    actorUserId: string | null
    type: string
    message: string
    createdAt: string
    isRead: boolean
    workspaceId: string | null
    entityId: string | null
}