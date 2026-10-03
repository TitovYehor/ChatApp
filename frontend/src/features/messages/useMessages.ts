import {
    useEffect,
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
    searchQuery: string,
) {
    const pageSize = 50

    const [
        pageNumber,
        setPageNumber,
    ] = useState(1)

    const normalizedSearchQuery =
        searchQuery.trim()

    useEffect(() => {
        setPageNumber(1)
    }, [
        channelId,
        normalizedSearchQuery,
    ])

    const query = useQuery({
        queryKey: [
            'messages',
            channelId,
            pageNumber,
            pageSize,
            normalizedSearchQuery,
        ],
        queryFn: () =>
            getByChannelId(
                channelId!,
                {
                    pageNumber,
                    pageSize,
                    search:
                        normalizedSearchQuery ||
                        undefined,
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

    const totalCount =
        query.data?.totalCount ?? 0

    const totalPages =
        Math.max(
            1,
            Math.ceil(
                totalCount /
                pageSize,
            ),
        )

    return {
        messages:
            query.data?.items ?? [],

        pageNumber,

        pageSize,

        totalCount,

        totalPages,

        hasPreviousPage:
            pageNumber > 1,

        hasNextPage:
            pageNumber <
            totalPages,

        goToPreviousPage: () => {
            setPageNumber(
                (current) =>
                    Math.max(
                        1,
                        current - 1,
                    ),
            )
        },

        goToNextPage: () => {
            setPageNumber(
                (current) =>
                    Math.min(
                        totalPages,
                        current + 1,
                    ),
            )
        },

        isLoading:
            query.isLoading,

        isFetching:
            query.isFetching,

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