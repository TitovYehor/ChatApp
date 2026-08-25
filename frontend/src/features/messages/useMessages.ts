import { useMutation, useQuery } from '@tanstack/react-query'

import {
    create,
    getByChannelId,
} from '../../api/messageApi'

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

    const createMutation = useMutation({
        mutationFn: (content: string) =>
            create(channelId!, {
                content,
            }),
        onSuccess: () => {
            void query.refetch()
        },
    })

    return {
        messages: query.data?.items ?? [],

        isLoading: query.isLoading,

        error: query.error
            ? 'Failed to load messages'
            : null,

        reload: query.refetch,

        sendMessage: createMutation.mutateAsync,

        isSending: createMutation.isPending,

        sendError: createMutation.error
            ? 'Failed to send message'
            : null,
    }
}