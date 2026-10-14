import { useQuery } from '@tanstack/react-query'
import { getPublicProfile } from '../../api/userApi'

export function usePublicUserProfile(userId: string | undefined) {
    const query = useQuery({
        queryKey: ['public-user-profile', userId],
        queryFn: () => getPublicProfile(userId!),
        enabled: Boolean(userId),
    })

    return {
        profile: query.data ?? null,
        isLoading: query.isLoading,
        error: query.error
            ? 'Failed to load this user profile'
            : null,
    }
}