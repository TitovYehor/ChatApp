import { useQuery } from '@tanstack/react-query'

import { getByWorkspaceId } from '../../api/channelApi'

export function useChannels(
    workspaceId: string | null,
) {
    const query = useQuery({
        queryKey: [
            'channels',
            workspaceId,
        ],
        queryFn: () =>
            getByWorkspaceId(workspaceId!),
        enabled: workspaceId !== null,
    })

    return {
        channels: query.data ?? [],
        isLoading: query.isLoading,
        error: query.error
            ? 'Failed to load channels'
            : null,
        reload: query.refetch,
    }
}