import { useCallback, useEffect, useRef, useState } from 'react'
import * as friendsApi from '../../api/friendsApi'
import type {
    FriendResponse,
    FriendSearchResponse,
} from '../../types/friendTypes'

export function useFriends() {
    const [friends, setFriends] = useState<FriendResponse[]>([])
    const [query, setQuery] = useState('')
    const [results, setResults] = useState<FriendSearchResponse[]>([])
    const [isLoadingFriends, setIsLoadingFriends] = useState(true)
    const [isSearching, setIsSearching] = useState(false)
    const [addingUserId, setAddingUserId] = useState<string | null>(null)
    const [error, setError] = useState<string | null>(null)
    const [notice, setNotice] = useState<string | null>(null)

    const searchVersion = useRef(0)

    const loadFriends = useCallback(async () => {
        setIsLoadingFriends(true)
        setError(null)

        try {
            const data = await friendsApi.getAll()
            setFriends(data)
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : 'Could not load your friends.',
            )
        } finally {
            setIsLoadingFriends(false)
        }
    }, [])

    const updateQuery = useCallback((value: string) => {
        const trimmedValue = value.trim()

        setQuery(value)
        setNotice(null)
        setError(null)

        if (!trimmedValue) {
            setResults([])
            setIsSearching(false)
            searchVersion.current += 1
        }
    }, [])

    useEffect(() => {
        let cancelled = false

        async function loadInitialFriends() {
            try {
                const data = await friendsApi.getAll()

                if (!cancelled) {
                    setFriends(data)
                    setError(null)
                }
            } catch (err) {
                if (!cancelled) {
                    setError(
                        err instanceof Error
                            ? err.message
                            : 'Could not load your friends',
                    )
                }
            } finally {
                if (!cancelled) {
                    setIsLoadingFriends(false)
                }
            }
        }

        void loadInitialFriends()

        return () => {
            cancelled = true
        }
    }, [])

    useEffect(() => {
        const trimmedQuery = query.trim()

        if (!trimmedQuery) {
            return
        }

        const version = ++searchVersion.current

        const timeoutId = window.setTimeout(async () => {
            setIsSearching(true)

            try {
                const data = await friendsApi.search(trimmedQuery)

                if (searchVersion.current === version) {
                    setResults(data)
                    setError(null)
                }
            } catch (err) {
                if (searchVersion.current === version) {
                    setError(
                        err instanceof Error
                            ? err.message
                            : 'Could not search users',
                    )
                    setResults([])
                }
            } finally {
                if (searchVersion.current === version) {
                    setIsSearching(false)
                }
            }
        }, 300)

        return () => {
            window.clearTimeout(timeoutId)
        }
    }, [query])

    const addFriend = useCallback(async (user: FriendSearchResponse) => {
        setAddingUserId(user.userId)
        setError(null)
        setNotice(null)

        try {
            await friendsApi.add(user.userId)

            setResults(current =>
                current.map(result =>
                    result.userId === user.userId
                        ? { ...result, isFriend: true }
                        : result,
                ),
            )

            await loadFriends()
            setNotice(`You and ${user.username} are now friends.`)
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : 'Could not add this friend.',
            )
        } finally {
            setAddingUserId(null)
        }
    }, [loadFriends])

    return {
        friends,
        query,
        setQuery: updateQuery,
        results,
        isLoadingFriends,
        isSearching,
        addingUserId,
        error,
        notice,
        addFriend,
        refreshFriends: loadFriends,
    }
}