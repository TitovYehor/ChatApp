import { useQuery } from '@tanstack/react-query'

import { getByChannelId } from '../../api/messageApi'

export function useMessages(
    channelId: string | null,
) {
    const query = useQuery({
        queryKey: [
            'messages',
            channelId,
        ],
        queryFn: () =>
            getByChannelId(channelId!, {
                pageNumber: 1,
                pageSize: 50,
            }),
        enabled: channelId !== null,
    })

    return {
        messages: query.data?.items ?? [],
        isLoading: query.isLoading,
        error: query.error
            ? 'Failed to load messages'
            : null,
        reload: query.refetch,
    }
}