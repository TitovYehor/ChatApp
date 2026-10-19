import { useCallback, useEffect, useState } from 'react'

import * as friendsApi from '../../api/friendsApi'
import type { FriendResponse } from '../../types/friendTypes'

interface UseProfileFriendshipResult {
    isFriend: boolean
    isLoadingFriendship: boolean
    isUpdatingFriendship: boolean
    error: string | null
    notice: string | null
    toggleFriendship: () => Promise<void>
}

export function useProfileFriendship(
    profileUserId: string | undefined,
    profileUsername: string | undefined,
    isOwnProfile: boolean,
): UseProfileFriendshipResult {
    const [isFriend, setIsFriend] = useState(false)
    const [isLoadingFriendship, setIsLoadingFriendship] = useState(true)
    const [isUpdatingFriendship, setIsUpdatingFriendship] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [notice, setNotice] = useState<string | null>(null)

    useEffect(() => {
        let cancelled = false

        async function loadFriendship() {
            if (!profileUserId || isOwnProfile) {
                setIsFriend(false)
                setIsLoadingFriendship(false)
                return
            }

            setIsLoadingFriendship(true)
            setError(null)

            try {
                const friends: FriendResponse[] = await friendsApi.getAll()

                if (!cancelled) {
                    setIsFriend(
                        friends.some(friend => friend.userId === profileUserId),
                    )
                }
            } catch (err) {
                if (!cancelled) {
                    setError(
                        err instanceof Error
                            ? err.message
                            : 'Could not load friendship status.',
                    )
                }
            } finally {
                if (!cancelled) {
                    setIsLoadingFriendship(false)
                }
            }
        }

        void loadFriendship()

        return () => {
            cancelled = true
        }
    }, [profileUserId, isOwnProfile])

    const toggleFriendship = useCallback(async () => {
        if (!profileUserId || !profileUsername || isOwnProfile) {
            return
        }

        setIsUpdatingFriendship(true)
        setError(null)
        setNotice(null)

        try {
            if (isFriend) {
                await friendsApi.remove(profileUserId)
                setIsFriend(false)
                setNotice(`${profileUsername} was removed from your friends.`)
            } else {
                await friendsApi.add(profileUserId)
                setIsFriend(true)
                setNotice(`You and ${profileUsername} are now friends.`)
            }
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : 'Could not update friendship.',
            )
        } finally {
            setIsUpdatingFriendship(false)
        }
    }, [profileUserId, profileUsername, isOwnProfile, isFriend])

    return {
        isFriend,
        isLoadingFriendship,
        isUpdatingFriendship,
        error,
        notice,
        toggleFriendship,
    }
}