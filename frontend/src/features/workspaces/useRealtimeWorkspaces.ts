import {
    useEffect,
} from 'react'

import {
    useQueryClient,
} from '@tanstack/react-query'

import {
    getChatConnection,
} from '../../services/signalR/chatConnection'

import type {
    WorkspaceResponse,
    WorkspaceUpdatedResponse,
} from '../../types/workspaceTypes'

export function useRealtimeWorkspaces() {
    const queryClient =
        useQueryClient()

    const connection =
        getChatConnection()

    useEffect(() => {
        const handleWorkspaceUpdated = (
            response: WorkspaceUpdatedResponse,
        ) => {
            queryClient.setQueryData<
                WorkspaceResponse[]
            >(
                ['workspaces'],
                (current) => {
                    if (!current) {
                        return current
                    }

                    const workspaceExists =
                        current.some(
                            (workspace) =>
                                workspace.id ===
                                response.workspaceId,
                        )

                    if (!workspaceExists) {
                        return current
                    }

                    return current.map(
                        (workspace) =>
                            workspace.id ===
                                response.workspaceId
                                ? {
                                    ...workspace,
                                    name:
                                        response.name,
                                    description:
                                        response.description,
                                }
                                : workspace,
                    )
                },
            )

            const currentWorkspace =
                queryClient.getQueryData<
                    WorkspaceResponse
                >([
                    'workspace',
                    response.workspaceId,
                ])

            if (currentWorkspace) {
                queryClient.setQueryData<
                    WorkspaceResponse
                >(
                    [
                        'workspace',
                        response.workspaceId,
                    ],
                    {
                        ...currentWorkspace,
                        name:
                            response.name,
                        description:
                            response.description,
                    },
                )
            }
        }

        connection.on(
            'WorkspaceUpdated',
            handleWorkspaceUpdated,
        )

        return () => {
            connection.off(
                'WorkspaceUpdated',
                handleWorkspaceUpdated,
            )
        }
    }, [
        connection,
        queryClient,
    ])
}