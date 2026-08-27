import { useQuery } from '@tanstack/react-query'

import type {
    OnlineUserResponse,
} from '../../types/presenceTypes'

export function useOnlineUsers() {
    const query = useQuery<
        OnlineUserResponse[]
    >({
        queryKey: [
            'presence',
            'online-users',
        ],
        queryFn: async () => {
            return []
        },
        initialData: [],
        staleTime: Infinity,
    })

    return {
        onlineUsers:
            query.data ?? [],

        isLoading:
            query.isLoading,
    }
}