import { useEffect } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { getChatConnection } from '../../services/signalR/chatConnection'
import type {
    WorkspaceResponse,
    WorkspaceUpdatedResponse,
    WorkspaceDeletedResponse,
    WorkspaceMemberAddedResponse,
} from '../../types/workspaceTypes'

import {
    SignalREvents
} from '../../types/signalREvents'

export function useRealtimeWorkspaces(
    selectedWorkspaceId: string | null,
    onWorkspaceDeleted: (workspaceId: string) => void,
) {
    const queryClient = useQueryClient()
    const connection = getChatConnection()

    useEffect(() => {
        const handleWorkspaceUpdated = (
            response: WorkspaceUpdatedResponse,
        ) => {
            queryClient.setQueryData<WorkspaceResponse[]>(
                ['workspaces'],
                (current) => {
                    if (!current) return current

                    const workspaceExists = current.some(
                        (workspace) =>
                            workspace.id === response.workspaceId,
                    )

                    if (!workspaceExists) {
                        return current
                    }

                    return current.map((workspace) =>
                        workspace.id === response.workspaceId
                            ? {
                                ...workspace,
                                name: response.name,
                                description: response.description,
                            }
                            : workspace,
                    )
                },
            )

            const currentWorkspace =
                queryClient.getQueryData<WorkspaceResponse>(
                    ['workspace', response.workspaceId],
                )

            if (currentWorkspace) {
                queryClient.setQueryData<WorkspaceResponse>(
                    ['workspace', response.workspaceId],
                    {
                        ...currentWorkspace,
                        name: response.name,
                        description: response.description,
                    },
                )
            }
        }

        const handleWorkspaceDeleted = (
            response: WorkspaceDeletedResponse,
        ) => {
            queryClient.setQueryData<WorkspaceResponse[]>(
                ['workspaces'],
                (current) => {
                    if (!current) return current

                    return current.filter(
                        (workspace) =>
                            workspace.id !== response.workspaceId,
                    )
                },
            )

            queryClient.removeQueries({
                queryKey: ['workspace', response.workspaceId],
            })

            queryClient.removeQueries({
                queryKey: [
                    'workspace-members',
                    response.workspaceId,
                ],
            })

            if (
                selectedWorkspaceId ===
                response.workspaceId
            ) {
                onWorkspaceDeleted(response.workspaceId)
            }
        }

        const handleWorkspaceMemberAdded = (
            response: WorkspaceMemberAddedResponse,
        ) => {
            queryClient.setQueryData<WorkspaceResponse[]>(
                ['workspaces'],
                (current) => {
                    if (!current) {
                        return [
                            {
                                id: response.workspaceId,
                                name: response.name,
                                description: response.description,
                                currentUserRole:
                                    response.currentUserRole,
                                createdAt: response.createdAt,
                            },
                        ]
                    }

                    const alreadyExists = current.some(
                        (workspace) =>
                            workspace.id === response.workspaceId,
                    )

                    if (alreadyExists) {
                        return current
                    }

                    return [
                        ...current,
                        {
                            id: response.workspaceId,
                            name: response.name,
                            description: response.description,
                            currentUserRole:
                                response.currentUserRole,
                            createdAt: response.createdAt,
                        },
                    ]
                },
            )
        }

        connection.on(
            SignalREvents.WorkspaceUpdated,
            handleWorkspaceUpdated,
        )

        connection.on(
            SignalREvents.WorkspaceDeleted,
            handleWorkspaceDeleted,
        )

        connection.on(
            SignalREvents.WorkspaceMemberAdded,
            handleWorkspaceMemberAdded,
        )

        return () => {
            connection.off(
                SignalREvents.WorkspaceUpdated,
                handleWorkspaceUpdated,
            )

            connection.off(
                SignalREvents.WorkspaceDeleted,
                handleWorkspaceDeleted,
            )

            connection.off(
                SignalREvents.WorkspaceMemberAdded,
                handleWorkspaceMemberAdded,
            )
        }
    }, [
        connection,
        onWorkspaceDeleted,
        queryClient,
        selectedWorkspaceId,
    ])
}