import {
    useState,
} from 'react'

import {
    useMutation,
    useQuery,
    useQueryClient,
} from '@tanstack/react-query'

import {
    create,
    getByWorkspaceId,
    update,
    remove,
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

    const [
        updateErrorChannelId,
        setUpdateErrorChannelId,
    ] = useState<string | null>(null)

    const updateMutation =
        useMutation({
            mutationFn: ({
                channelId,
                name,
            }: {
                channelId: string
                name: string
            }) =>
                update(
                    channelId,
                    {
                        name,
                    },
                ),

            onMutate: ({
                channelId,
            }) => {
                setUpdateErrorChannelId(
                    null,
                )

                return {
                    channelId,
                }
            },

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
                            return current
                        }

                        return current.map(
                            (
                                currentChannel,
                            ) =>
                                currentChannel.id ===
                                    channel.id
                                    ? channel
                                    : currentChannel,
                        )
                    },
                )
            },

            onError: (
                _error,
                _variables,
                context,
            ) => {
                setUpdateErrorChannelId(
                    context?.channelId ??
                    null,
                )
            },
        })

    const [
        deleteErrorChannelId,
        setDeleteErrorChannelId,
    ] = useState<string | null>(null)

    const deleteMutation =
        useMutation({
            mutationFn: (
                channelId: string,
            ) =>
                remove(
                    channelId,
                ),

            onMutate: (
                channelId,
            ) => {
                setDeleteErrorChannelId(
                    null,
                )

                return {
                    channelId,
                }
            },

            onSuccess: (
                _data,
                channelId,
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
                            return current
                        }

                        return current.filter(
                            (
                                channel,
                            ) =>
                                channel.id !==
                                channelId,
                        )
                    },
                )
            },

            onError: (
                _error,
                _variables,
                context,
            ) => {
                setDeleteErrorChannelId(
                    context?.channelId ??
                    null,
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

        updateChannel:
            updateMutation.mutateAsync,

        updatingChannelId:
            updateMutation.isPending
                ? updateMutation
                    .variables
                    ?.channelId ?? null
                : null,

        isUpdating:
            updateMutation.isPending,

        updateErrorChannelId,

        updateChannelError:
            updateMutation.error
                ? 'Failed to update channel'
                : null,

        deleteChannel:
            deleteMutation.mutateAsync,

        deletingChannelId:
            deleteMutation.isPending
                ? deleteMutation.variables ??
                null
                : null,

        isDeleting:
            deleteMutation.isPending,

        deleteErrorChannelId,

        deleteChannelError:
            deleteMutation.error
                ? 'Failed to delete channel'
                : null,
    }
}