import {
    useMutation,
    useQuery,
} from '@tanstack/react-query'

import {
    create,
    getByChannelId,
    remove,
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

    const createMutation =
        useMutation({
            mutationFn: (
                content: string,
            ) =>
                create(channelId!, {
                    content,
                }),
        })

    const deleteMutation =
        useMutation({
            mutationFn: (
                messageId: string,
            ) =>
                remove(messageId),
        })

    return {
        messages:
            query.data?.items ?? [],

        isLoading:
            query.isLoading,

        error:
            query.error
                ? 'Failed to load messages'
                : null,

        reload:
            query.refetch,

        sendMessage:
            createMutation.mutateAsync,

        isSending:
            createMutation.isPending,

        sendError:
            createMutation.error
                ? 'Failed to send message'
                : null,

        deleteMessage:
            deleteMutation.mutateAsync,

        isDeleting:
            deleteMutation.isPending,

        deleteError:
            deleteMutation.error
                ? 'Failed to delete message'
                : null,
    }
}