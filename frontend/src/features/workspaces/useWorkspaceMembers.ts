import {
    useMutation,
    useQuery,
    useQueryClient,
} from '@tanstack/react-query'

import {
    addMember,
    changeMemberRole,
    getMembers,
    removeMember,
    transferOwnership,
    searchMembers,
} from '../../api/workspaceApi'

import type {
    WorkspaceRole,
} from '../../types/workspaceTypes'

export function useWorkspaceMembers(
    workspaceId: string | null,
    searchQuery: string,
) {
    const queryClient = useQueryClient()

    const query = useQuery({
        queryKey: [
            'workspace-members',
            workspaceId,
        ],
        queryFn: () =>
            getMembers(
                workspaceId!,
            ),
        enabled:
            workspaceId !== null,
    })

    const normalizedSearchQuery = searchQuery.trim()

    const searchQueryResult = useQuery({
        queryKey: [
            'workspace-members',
            workspaceId,
            'search',
            normalizedSearchQuery,
        ],
        queryFn: () =>
            searchMembers(
                workspaceId!,
                {
                    query: normalizedSearchQuery,
                },
            ),
        enabled:
            workspaceId !== null &&
            normalizedSearchQuery.length > 0,
    })

    const addMemberMutation =
        useMutation({
            mutationFn: (
                usernameOrEmail: string,
            ) =>
                addMember(
                    workspaceId!,
                    {
                        usernameOrEmail,
                    },
                ),

            onSuccess: async () => {
                await queryClient.invalidateQueries({
                    queryKey: [
                        'workspace-members',
                        workspaceId,
                    ],
                })
            },
        })

    const removeMemberMutation =
        useMutation({
            mutationFn: (
                usernameOrEmail: string,
            ) =>
                removeMember(
                    workspaceId!,
                    {
                        usernameOrEmail,
                    },
                ),

            onSuccess: async () => {
                await queryClient.invalidateQueries({
                    queryKey: [
                        'workspace-members',
                        workspaceId,
                    ],
                })
            },
        })

    const changeMemberRoleMutation =
        useMutation({
            mutationFn: ({
                usernameOrEmail,
                role,
            }: {
                usernameOrEmail: string
                role: WorkspaceRole
            }) =>
                changeMemberRole(
                    workspaceId!,
                    {
                        usernameOrEmail,
                        role,
                    },
                ),

            onSuccess: async () => {
                await queryClient.invalidateQueries({
                    queryKey: [
                        'workspace-members',
                        workspaceId,
                    ],
                })
            },
        })

    const transferOwnershipMutation =
        useMutation({
            mutationFn: (
                usernameOrEmail: string,
            ) =>
                transferOwnership(
                    workspaceId!,
                    {
                        usernameOrEmail,
                    },
                ),

            onSuccess: async () => {
                await Promise.all([
                    queryClient.invalidateQueries({
                        queryKey: [
                            'workspace-members',
                            workspaceId,
                        ],
                    }),

                    queryClient.invalidateQueries({
                        queryKey: [
                            'workspaces',
                        ],
                    }),

                    queryClient.invalidateQueries({
                        queryKey: [
                            'workspace',
                            workspaceId,
                        ],
                    }),
                ])
            },
        })

    return {
        members:
            query.data ?? [],

        searchResults:
            normalizedSearchQuery.length > 0
                ? searchQueryResult.data ?? []
                : [],

        isSearching:
            normalizedSearchQuery.length > 0 &&
            searchQueryResult.isFetching,

        searchError:
            searchQueryResult.isError
                ? 'Failed to search workspace members'
                : null,

        isLoading:
            query.isLoading,

        error: query.error
            ? 'Failed to load workspace members'
            : null,

        reload:
            query.refetch,

        addMember:
            addMemberMutation.mutateAsync,

        isAdding:
            addMemberMutation.isPending,

        addError:
            addMemberMutation.error
                ? 'Failed to add workspace member'
                : null,

        removeMember:
            removeMemberMutation.mutateAsync,

        removingMember:
            removeMemberMutation.isPending
                ? removeMemberMutation.variables ??
                null
                : null,

        isRemoving:
            removeMemberMutation.isPending,

        removeError:
            removeMemberMutation.error
                ? 'Failed to remove workspace member'
                : null,

        changeMemberRole:
            changeMemberRoleMutation.mutateAsync,

        changingMemberRole:
            changeMemberRoleMutation.isPending
                ? changeMemberRoleMutation.variables?.usernameOrEmail ??
                null
                : null,

        isChangingMemberRole:
            changeMemberRoleMutation.isPending,

        changeMemberRoleError:
            changeMemberRoleMutation.error
                ? 'Failed to change member role'
                : null,

        transferOwnership:
            transferOwnershipMutation.mutateAsync,

        isTransferringOwnership:
            transferOwnershipMutation.isPending,

        transferringOwnership:
            transferOwnershipMutation.isPending
                ? transferOwnershipMutation.variables ??
                null
                : null,

        transferOwnershipError:
            transferOwnershipMutation.error
                ? 'Failed to transfer workspace ownership'
                : null,
    }
}