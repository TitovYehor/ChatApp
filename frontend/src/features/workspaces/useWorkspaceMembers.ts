import {
    useMutation,
    useQuery,
    useQueryClient,
} from '@tanstack/react-query'

import {
    addMember,
    getMembers,
    removeMember,
} from '../../api/workspaceApi'

export function useWorkspaceMembers(
    workspaceId: string | null,
) {
    const queryClient =
        useQueryClient()

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

    return {
        members:
            query.data ?? [],

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
    }
}