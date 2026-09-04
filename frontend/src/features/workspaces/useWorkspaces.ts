import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

import {
    create,
    getAll,
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
    }
}