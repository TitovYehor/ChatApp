import {
    useState,
} from 'react'

import {
    useMutation,
    useQuery,
    useQueryClient,
} from '@tanstack/react-query'

import {
    create,
    getAll,
    remove,
    update,
} from '../../api/workspaceApi'

import type {
    WorkspaceResponse,
} from '../../types/workspaceTypes'

export function useWorkspaces() {
    const queryClient =
        useQueryClient()

    const query = useQuery({
        queryKey: ['workspaces'],
        queryFn: getAll,
    })

    const createMutation =
        useMutation({
            mutationFn: ({
                name,
                description,
            }: {
                name: string
                description: string
            }) =>
                create({
                    name,
                    description,
                }),

            onSuccess: (
                workspace,
            ) => {
                queryClient.setQueryData<
                    WorkspaceResponse[]
                >(
                    ['workspaces'],
                    (
                        current,
                    ) => {
                        if (!current) {
                            return [
                                workspace,
                            ]
                        }

                        return [
                            ...current,
                            workspace,
                        ]
                    },
                )
            },
        })

    const [
        updateErrorWorkspaceId,
        setUpdateErrorWorkspaceId,
    ] = useState<string | null>(null)

    const updateMutation =
        useMutation({
            mutationFn: ({
                workspaceId,
                name,
                description,
            }: {
                workspaceId: string
                name: string
                description: string
            }) =>
                update(
                    workspaceId,
                    {
                        name,
                        description,
                    },
                ),

            onMutate: ({
                workspaceId,
            }) => {
                setUpdateErrorWorkspaceId(
                    null,
                )

                return {
                    workspaceId,
                }
            },

            onSuccess: (
                workspace,
            ) => {
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
                                currentWorkspace,
                            ) =>
                                currentWorkspace.id ===
                                    workspace.id
                                    ? workspace
                                    : currentWorkspace,
                        )
                    },
                )

                queryClient.setQueryData(
                    [
                        'workspace',
                        workspace.id,
                    ],
                    workspace,
                )
            },

            onError: (
                _error,
                _variables,
                context,
            ) => {
                setUpdateErrorWorkspaceId(
                    context?.workspaceId ??
                    null,
                )
            },
        })

    const [
        deleteErrorWorkspaceId,
        setDeleteErrorWorkspaceId,
    ] = useState<string | null>(null)

    const deleteMutation =
        useMutation({
            mutationFn: (
                workspaceId: string,
            ) =>
                remove(
                    workspaceId,
                ),

            onMutate: (
                workspaceId,
            ) => {
                setDeleteErrorWorkspaceId(
                    null,
                )

                return {
                    workspaceId,
                }
            },

            onSuccess: (
                _data,
                workspaceId,
            ) => {
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

                        return current.filter(
                            (
                                workspace,
                            ) =>
                                workspace.id !==
                                workspaceId,
                        )
                    },
                )

                queryClient.removeQueries({
                    queryKey: [
                        'workspace',
                        workspaceId,
                    ],
                })

                queryClient.removeQueries({
                    queryKey: [
                        'workspace-members',
                        workspaceId,
                    ],
                })
            },

            onError: (
                _error,
                _variables,
                context,
            ) => {
                setDeleteErrorWorkspaceId(
                    context?.workspaceId ??
                    null,
                )
            },
        })

    return {
        workspaces:
            query.data ?? [],

        isLoading:
            query.isLoading,

        error: query.error
            ? 'Failed to load workspaces'
            : null,

        reload:
            query.refetch,

        createWorkspace:
            createMutation.mutateAsync,

        isCreating:
            createMutation.isPending,

        createError:
            createMutation.error
                ? 'Failed to create workspace'
                : null,

        updateWorkspace:
            updateMutation.mutateAsync,

        updatingWorkspaceId:
            updateMutation.isPending
                ? updateMutation.variables
                    ?.workspaceId ??
                null
                : null,

        isUpdating:
            updateMutation.isPending,

        updateErrorWorkspaceId,

        updateWorkspaceError:
            updateMutation.error
                ? 'Failed to update workspace'
                : null,

        deleteWorkspace:
            deleteMutation.mutateAsync,

        deletingWorkspaceId:
            deleteMutation.isPending
                ? deleteMutation.variables ??
                null
                : null,

        isDeleting:
            deleteMutation.isPending,

        deleteErrorWorkspaceId,

        deleteWorkspaceError:
            deleteMutation.error
                ? 'Failed to delete workspace'
                : null,
    }
}