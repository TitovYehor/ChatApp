import {
    useMutation,
    useQuery,
    useQueryClient,
} from '@tanstack/react-query'

import * as privateConversationApi from '../../api/privateConversationApi'

import {
    privateConversationKeys,
} from './usePrivateConversations'

export function usePrivateMessages(
    conversationId: string | undefined,
    pageNumber = 1,
    pageSize = 50,
) {
    const query = useQuery({
        queryKey: conversationId
            ? privateConversationKeys.messages(
                conversationId,
                pageNumber,
                pageSize,
            )
            : ['private-messages', null, pageNumber, pageSize],
        queryFn: () =>
            privateConversationApi.getMessages(
                conversationId!,
                pageNumber,
                pageSize,
            ),
        enabled: Boolean(conversationId),
    })

    return {
        result: query.data,
        isLoading: query.isLoading,
        isFetching: query.isFetching,
        error: query.error
            ? 'Failed to load private messages'
            : null,
        refetch: query.refetch,
    }
}

export function useSendPrivateMessage(
    conversationId: string | undefined,
) {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (content: string) => {
            if (!conversationId) {
                throw new Error('No conversation is selected')
            }

            return privateConversationApi.sendMessage(
                conversationId,
                { content },
            )
        },
        onSuccess: async () => {
            if (!conversationId) {
                return
            }

            await Promise.all([
                queryClient.invalidateQueries({
                    queryKey: [
                        'private-messages',
                        conversationId,
                    ],
                }),
                queryClient.invalidateQueries({
                    queryKey: privateConversationKeys.all,
                }),
            ])
        },
    })
}