import {
    useMutation,
    useQuery,
    useQueryClient,
} from '@tanstack/react-query'

import {
    create,
    getByWorkspaceId,
} from '../../api/channelApi'

import type {
    ChannelResponse,
} from '../../types/channelTypes'

export function useChannels(
    workspaceId: string | null,
) {
    const queryClient =
        useQueryClient()

    const query = useQuery({
        queryKey: [
            'channels',
            workspaceId,
        ],
        queryFn: () =>
            getByWorkspaceId(
                workspaceId!,
            ),
        enabled:
            workspaceId !== null,
    })

    const createMutation =
        useMutation({
            mutationFn: (
                name: string,
            ) =>
                create(
                    workspaceId!,
                    {
                        name,
                    },
                ),

            onSuccess: (
                channel,
            ) => {
                queryClient.setQueryData<
                    ChannelResponse[]
                >(
                    [
                        'channels',
                        workspaceId,
                    ],
                    (
                        current,
                    ) => {
                        if (!current) {
                            return [
                                channel,
                            ]
                        }

                        return [
                            ...current,
                            channel,
                        ]
                    },
                )
            },
        })

    return {
        channels:
            query.data ?? [],

        isLoading:
            query.isLoading,

        error: query.error
            ? 'Failed to load channels'
            : null,

        reload:
            query.refetch,

        createChannel:
            createMutation.mutateAsync,

        isCreating:
            createMutation.isPending,

        createError:
            createMutation.error
                ? 'Failed to create channel'
                : null,
    }
}