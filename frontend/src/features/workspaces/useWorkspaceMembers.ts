import { useQuery } from '@tanstack/react-query'

import {
    getMembers,
} from '../../api/workspaceApi'

export function useWorkspaceMembers(
    workspaceId: string | null,
) {
    const query = useQuery({
        queryKey: [
            'workspace-members',
            workspaceId,
        ],
        queryFn: () =>
            getMembers(workspaceId!),
        enabled: workspaceId !== null,
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
    }
}