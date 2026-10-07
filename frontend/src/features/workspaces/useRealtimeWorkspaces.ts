import {
    useEffect,
    useCallback,
} from 'react'
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
    WorkspaceLeftResponse,
    WorkspaceRole,
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

    const updateWorkspaceSearchCaches = useCallback(
        (
            workspace: WorkspaceResponse,
        ) => {
            const searchQueries =
                queryClient.getQueriesData<
                    WorkspaceResponse[]
                >({
                    queryKey: [
                        'workspaces',
                        'search',
                    ],
                })

            searchQueries.forEach(
                ([queryKey, current]) => {
                    if (!current) {
                        return
                    }

                    const searchQuery =
                        String(
                            queryKey[2] ?? '',
                        )
                            .trim()
                            .toLowerCase()

                    if (!searchQuery) {
                        return
                    }

                    const matches =
                        workspace.name
                            .toLowerCase()
                            .includes(searchQuery)

                    const exists =
                        current.some(
                            (item) =>
                                item.id ===
                                workspace.id,
                        )

                    if (!matches) {
                        queryClient.setQueryData<
                            WorkspaceResponse[]
                        >(
                            queryKey,
                            current.filter(
                                (item) =>
                                    item.id !==
                                    workspace.id,
                            ),
                        )

                        return
                    }

                    if (exists) {
                        queryClient.setQueryData<
                            WorkspaceResponse[]
                        >(
                            queryKey,
                            current.map(
                                (item) =>
                                    item.id ===
                                        workspace.id
                                        ? workspace
                                        : item,
                            ),
                        )

                        return
                    }

                    queryClient.setQueryData<
                        WorkspaceResponse[]
                    >(
                        queryKey,
                        [
                            ...current,
                            workspace,
                        ],
                    )
                },
            )
        },
        [queryClient],
    )

    const removeWorkspaceFromSearchCaches = useCallback(
        (
            workspaceId: string,
        ) => {
            const searchQueries =
                queryClient.getQueriesData<
                    WorkspaceResponse[]
                >({
                    queryKey: [
                        'workspaces',
                        'search',
                    ],
                })

            searchQueries.forEach(
                ([queryKey, current]) => {
                    if (!current) {
                        return
                    }

                    queryClient.setQueryData<
                        WorkspaceResponse[]
                    >(
                        queryKey,
                        current.filter(
                            (workspace) =>
                                workspace.id !==
                                workspaceId,
                        ),
                    )
                },
            )
        },
        [queryClient],
    )

    const invalidateWorkspaceMemberSearchCaches =
        useCallback(
            (workspaceId: string) => {
                void queryClient.invalidateQueries({
                    queryKey: [
                        'workspace-members',
                        workspaceId,
                        'search',
                    ],
                })
            },
            [queryClient],
        )

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
                queryClient.getQueryData<WorkspaceResponse>(
                    [
                        'workspace',
                        response.workspaceId,
                    ],
                )

            if (currentWorkspace) {
                const updatedWorkspace = {
                    ...currentWorkspace,
                    name: response.name,
                    description:
                        response.description,
                }

                queryClient.setQueryData<WorkspaceResponse>(
                    [
                        'workspace',
                        response.workspaceId,
                    ],
                    updatedWorkspace,
                )
            }

            void queryClient.invalidateQueries({
                queryKey: [
                    'workspaces',
                    'search',
                ],
            })
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

            removeWorkspaceFromSearchCaches(
                response.workspaceId,
            )
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

            invalidateWorkspaceMemberSearchCaches(
                response.workspaceId,
            )

            if (response.userId !== currentUserId) {
                return
            }

            const addedWorkspace: WorkspaceResponse = {
                id: response.workspaceId,
                name: response.name,
                description: response.description,
                currentUserRole: response.role,
                createdAt: response.createdAt,
            }

            queryClient.setQueryData<WorkspaceResponse[]>(
                ['workspaces'],
                (current) => {
                    if (!current) {
                        return [addedWorkspace]
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
                        addedWorkspace,
                    ]
                },
            )

            void queryClient.invalidateQueries({
                queryKey: [
                    'workspaces',
                    'search',
                ],
            })
        }

        const handleWorkspaceMemberRemoved = (
            response: WorkspaceMemberRemovedResponse,
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

                    return current.filter(
                        (member) =>
                            member.userId !==
                            response.userId,
                    )
                },
            )

            invalidateWorkspaceMemberSearchCaches(
                response.workspaceId,
            )

            if (response.userId !== currentUserId) {
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

            removeWorkspaceFromSearchCaches(
                response.workspaceId,
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
                (current) => {
                    if (!current) {
                        return current
                    }

                    return current.map(
                        (member) =>
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

            invalidateWorkspaceMemberSearchCaches(
                response.workspaceId,
            )

            if (response.userId !== currentUserId) {
                return
            }

            queryClient.setQueryData<
                WorkspaceResponse[]
            >(
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

            if (currentWorkspace) {
                const updatedWorkspace = {
                    ...currentWorkspace,
                    currentUserRole:
                        response.role,
                }

                queryClient.setQueryData<WorkspaceResponse>(
                    [
                        'workspace',
                        response.workspaceId,
                    ],
                    updatedWorkspace,
                )
            }

            void queryClient.invalidateQueries({
                queryKey: [
                    'workspaces',
                    'search',
                ],
            })
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

            invalidateWorkspaceMemberSearchCaches(
                response.workspaceId,
            )

            if (
                currentUserId !==
                response.previousOwnerUserId &&
                currentUserId !==
                response.newOwnerUserId
            ) {
                return
            }

            const currentUserRole: WorkspaceRole =
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

            if (currentWorkspace) {
                const updatedWorkspace = {
                    ...currentWorkspace,
                    currentUserRole,
                }

                queryClient.setQueryData<WorkspaceResponse>(
                    [
                        'workspace',
                        response.workspaceId,
                    ],
                    updatedWorkspace,
                )
            }

            void queryClient.invalidateQueries({
                queryKey: [
                    'workspaces',
                    'search',
                ],
            })
        }

        const handleWorkspaceLeft = (
            response: WorkspaceLeftResponse,
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

                    return current.filter(
                        (member) =>
                            member.userId !== response.userId,
                    )
                },
            )

            invalidateWorkspaceMemberSearchCaches(
                response.workspaceId,
            )

            if (response.userId !== currentUserId) {
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
                            workspace.id !== response.workspaceId,
                    )
                },
            )

            removeWorkspaceFromSearchCaches(
                response.workspaceId,
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

        connection.on(
            SignalREvents.WorkspaceLeft,
            handleWorkspaceLeft,
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

            connection.off(
                SignalREvents.WorkspaceLeft,
                handleWorkspaceLeft,
            )
        }
    }, [
        connection,
        currentUserId,
        onWorkspaceAccessLost,
        queryClient,
        selectedWorkspaceId,
        updateWorkspaceSearchCaches,
        removeWorkspaceFromSearchCaches,
        invalidateWorkspaceMemberSearchCaches,
    ])
}