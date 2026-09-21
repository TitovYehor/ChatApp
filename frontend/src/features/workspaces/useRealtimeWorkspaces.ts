import { useEffect } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { getChatConnection } from '../../services/signalR/chatConnection'
import type {
    WorkspaceResponse,
    WorkspaceUpdatedResponse,
    WorkspaceDeletedResponse,
    WorkspaceMemberAddedResponse,
    WorkspaceMemberRemovedResponse,
    WorkspaceMemberRoleChangedResponse,
    WorkspaceMemberResponse,
    WorkspaceOwnershipTransferredResponse,
} from '../../types/workspaceTypes'

import {
    SignalREvents
} from '../../types/signalREvents'

export function useRealtimeWorkspaces(
    currentUserId: string | null,
    selectedWorkspaceId: string | null,
    onWorkspaceAccessLost: (workspaceId: string) => void,
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

            if (selectedWorkspaceId === response.workspaceId) {
                onWorkspaceAccessLost(response.workspaceId)
            }
        }

        const handleWorkspaceMemberAdded = (
            response: WorkspaceMemberAddedResponse,
        ) => {
            queryClient.setQueryData<WorkspaceMemberResponse[]>(
                [
                    'workspace-members',
                    response.workspaceId,
                ],
                (current) => {
                    if (!current) {
                        return current
                    }

                    const alreadyExists = current.some(
                        (member) =>
                            member.userId ===
                            response.userId,
                    )

                    if (alreadyExists) {
                        return current
                    }

                    return [
                        ...current,
                        {
                            userId: response.userId,
                            username: response.username,
                            email: response.email,
                            role: response.role,
                            joinedAt: response.joinedAt,
                        },
                    ]
                },
            )

            if (response.userId !== currentUserId) {
                return
            }

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
                                    response.role,
                                createdAt:
                                    response.createdAt,
                            },
                        ]
                    }

                    const alreadyExists = current.some(
                        (workspace) =>
                            workspace.id ===
                            response.workspaceId,
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
                                response.role,
                            createdAt:
                                response.createdAt,
                        },
                    ]
                },
            )
        }

        const handleWorkspaceMemberRemoved = (
            response: WorkspaceMemberRemovedResponse,
        ) => {
            if (response.userId !== currentUserId) {
                queryClient.setQueryData<
                    WorkspaceMemberResponse[]
                >(
                    [
                        'workspace-members',
                        response.workspaceId,
                    ],
                    (current) => {
                        if (!current) {
                            return current
                        }

                        return current.filter(
                            (member) =>
                                member.userId !==
                                response.userId,
                        )
                    },
                )

                return
            }

            queryClient.setQueryData<WorkspaceResponse[]>(
                ['workspaces'],
                (current) => {
                    if (!current) {
                        return current
                    }

                    return current.filter(
                        (workspace) =>
                            workspace.id !==
                            response.workspaceId,
                    )
                },
            )

            queryClient.removeQueries({
                queryKey: [
                    'workspace',
                    response.workspaceId,
                ],
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
                onWorkspaceAccessLost(
                    response.workspaceId,
                )
            }
        }

        const handleWorkspaceMemberRoleChanged = (
            response: WorkspaceMemberRoleChangedResponse,
        ) => {
            queryClient.setQueryData<
                WorkspaceMemberResponse[]
            >(
                [
                    'workspace-members',
                    response.workspaceId,
                ],
                (
                    current,
                ) => {
                    if (!current) {
                        return current
                    }

                    return current.map(
                        (
                            member,
                        ) =>
                            member.userId ===
                                response.userId
                                ? {
                                    ...member,
                                    role:
                                        response.role,
                                }
                                : member,
                    )
                },
            )

            if (response.userId !== currentUserId) {
                return
            }

            queryClient.setQueryData<
                WorkspaceResponse[]
            >(
                ['workspaces'],
                (
                    current,
                ) => {
                    if (!current) {
                        return current
                    }

                    return current.map(
                        (
                            workspace,
                        ) =>
                            workspace.id ===
                                response.workspaceId
                                ? {
                                    ...workspace,
                                    currentUserRole:
                                        response.role,
                                }
                                : workspace,
                    )
                },
            )

            const currentWorkspace =
                queryClient.getQueryData<WorkspaceResponse>(
                    [
                        'workspace',
                        response.workspaceId,
                    ],
                )

            if (!currentWorkspace) {
                return
            }

            queryClient.setQueryData<WorkspaceResponse>(
                [
                    'workspace',
                    response.workspaceId,
                ],
                {
                    ...currentWorkspace,
                    currentUserRole:
                        response.role,
                },
            )
        }

        const handleWorkspaceOwnershipTransferred = (
            response: WorkspaceOwnershipTransferredResponse,
        ) => {
            queryClient.setQueryData<WorkspaceMemberResponse[]>(
                [
                    'workspace-members',
                    response.workspaceId,
                ],
                (current) => {
                    if (!current) {
                        return current
                    }

                    return current.map(
                        (member) => {
                            if (
                                member.userId ===
                                response.previousOwnerUserId
                            ) {
                                return {
                                    ...member,
                                    role: 2,
                                }
                            }

                            if (
                                member.userId ===
                                response.newOwnerUserId
                            ) {
                                return {
                                    ...member,
                                    role: 1,
                                }
                            }

                            return member
                        },
                    )
                },
            )

            if (
                currentUserId !==
                response.previousOwnerUserId &&
                currentUserId !==
                response.newOwnerUserId
            ) {
                return
            }

            const currentUserRole =
                currentUserId ===
                    response.newOwnerUserId
                    ? 1
                    : 2

            queryClient.setQueryData<WorkspaceResponse[]>(
                ['workspaces'],
                (current) => {
                    if (!current) {
                        return current
                    }

                    return current.map(
                        (workspace) =>
                            workspace.id ===
                                response.workspaceId
                                ? {
                                    ...workspace,
                                    currentUserRole,
                                }
                                : workspace,
                    )
                },
            )

            const currentWorkspace =
                queryClient.getQueryData<WorkspaceResponse>(
                    [
                        'workspace',
                        response.workspaceId,
                    ],
                )

            if (!currentWorkspace) {
                return
            }

            queryClient.setQueryData<WorkspaceResponse>(
                [
                    'workspace',
                    response.workspaceId,
                ],
                {
                    ...currentWorkspace,
                    currentUserRole,
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

        connection.on(
            SignalREvents.WorkspaceMemberRemoved,
            handleWorkspaceMemberRemoved,
        )

        connection.on(
            SignalREvents.WorkspaceMemberRoleChanged,
            handleWorkspaceMemberRoleChanged,
        )

        connection.on(
            SignalREvents.WorkspaceOwnershipTransferred,
            handleWorkspaceOwnershipTransferred,
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

            connection.off(
                SignalREvents.WorkspaceMemberRemoved,
                handleWorkspaceMemberRemoved,
            )

            connection.off(
                SignalREvents.WorkspaceMemberRoleChanged,
                handleWorkspaceMemberRoleChanged,
            )

            connection.off(
                SignalREvents.WorkspaceOwnershipTransferred,
                handleWorkspaceOwnershipTransferred,
            )
        }
    }, [
        connection,
        currentUserId,
        onWorkspaceAccessLost,
        queryClient,
        selectedWorkspaceId,
    ])
}