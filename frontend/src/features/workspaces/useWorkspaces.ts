import { useQuery } from '@tanstack/react-query'

import { getAll } from '../../api/workspaceApi'

export function useWorkspaces() {
    const query = useQuery({
        queryKey: ['workspaces'],
        queryFn: getAll,
    })

    return {
        workspaces: query.data ?? [],
        isLoading: query.isLoading,
        error: query.error
            ? 'Failed to load workspaces'
            : null,
        reload: query.refetch,
    }
}