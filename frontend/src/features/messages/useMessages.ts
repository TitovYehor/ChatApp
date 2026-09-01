import {
    useState,
} from 'react'

import {
    useMutation,
    useQuery,
} from '@tanstack/react-query'

import {
    create,
    getByChannelId,
    remove,
    update,
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
            getByChannelId(
                channelId!,
                {
                    pageNumber: 1,
                    pageSize: 50,
                },
            ),
        enabled:
            channelId !== null,
    })

    const createMutation = useMutation({
        mutationFn: (
            content: string,
        ) =>
            create(
                channelId!,
                {
                    content,
                },
            ),
    })

    const [
        updateErrorMessageId,
        setUpdateErrorMessageId,
    ] = useState<string | null>(null)

    const [
        deleteErrorMessageId,
        setDeleteErrorMessageId,
    ] = useState<string | null>(null)

    const updateMutation = useMutation({
        mutationFn: ({
            messageId,
            content,
        }: {
            messageId: string
            content: string
        }) =>
            update(
                messageId,
                {
                    content,
                },
            ),
        onMutate: ({
            messageId,
        }) => {
            setUpdateErrorMessageId(
                null,
            )

            return {
                messageId,
            }
        },
        onError: (
            _error,
            _variables,
            context,
        ) => {
            setUpdateErrorMessageId(
                context?.messageId ??
                null,
            )
        },
    })

    const deleteMutation = useMutation({
        mutationFn: (
            messageId: string,
        ) => remove(messageId),
        onMutate: (
            messageId,
        ) => {
            setDeleteErrorMessageId(
                null,
            )

            return {
                messageId,
            }
        },
        onError: (
            _error,
            _variables,
            context,
        ) => {
            setDeleteErrorMessageId(
                context?.messageId ??
                null,
            )
        },
    })

    return {
        messages:
            query.data?.items ?? [],

        isLoading:
            query.isLoading,

        error: query.error
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

        updateMessage:
            updateMutation.mutateAsync,

        updatingMessageId:
            updateMutation.variables
                ?.messageId ?? null,

        isUpdating:
            updateMutation.isPending,

        updateErrorMessageId,

        updateError:
            updateMutation.error
                ? 'Failed to update message'
                : null,
        
        deleteMessage:
            deleteMutation.mutateAsync,

        deletingMessageId:
            deleteMutation.variables ??
            null,

        isDeleting:
            deleteMutation.isPending,

        deleteErrorMessageId,

        deleteError:
            deleteMutation.error
                ? 'Failed to delete message'
                : null,
    }
}