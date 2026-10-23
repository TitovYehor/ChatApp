import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

import * as privateConversationApi from '../../api/privateConversationApi'

export const privateConversationKeys = {
    all: ['private-conversations'] as const,
    messages: (
        conversationId: string,
        pageNumber: number,
        pageSize: number,
    ) => [
        'private-messages',
        conversationId,
        pageNumber,
        pageSize,
    ] as const,
}

export function usePrivateConversations() {
    const query = useQuery({
        queryKey: privateConversationKeys.all,
        queryFn: privateConversationApi.getAll,
    })

    return {
        conversations: query.data ?? [],
        isLoading: query.isLoading,
        isFetching: query.isFetching,
        error: query.error
            ? 'Failed to load private conversations'
            : null,
        refetch: query.refetch,
    }
}

export function useGetOrCreatePrivateConversation() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (otherUserId: string) =>
            privateConversationApi.getOrCreate(otherUserId),
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: privateConversationKeys.all,
            })
        },
    })
}