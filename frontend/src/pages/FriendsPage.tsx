import ChatHeader from '../layouts/ChatHeader'
import { useAuth } from '../features/auth/useAuth'
import { useFriends } from '../features/friends/useFriends'
import type { FriendSearchResponse } from '../types/friendTypes'

import './css/FriendsPage.css'

function FriendsPage() {
    const { user, logout } = useAuth()

    const {
        friends,
        query,
        setQuery,
        results,
        isLoadingFriends,
        isSearching,
        addingUserId,
        error,
        notice,
        addFriend,
    } = useFriends()

    function renderSearchResult(friend: FriendSearchResponse) {
        return (
            <div
                className="friends-page__person"
                key={friend.userId}
            >
                <div className="friends-page__person-avatar">
                    {friend.username.charAt(0).toUpperCase()}
                </div>

                <div className="friends-page__person-info">
                    <span className="friends-page__person-name">
                        {friend.username}
                    </span>
                    <span className="friends-page__person-meta">
                        {friend.isFriend ? 'Already friends' : 'Chat App user'}
                    </span>
                </div>

                {friend.isFriend ? (
                    <span className="friends-page__added">
                        Friends
                    </span>
                ) : (
                    <button
                        type="button"
                        className="friends-page__add-button"
                        disabled={addingUserId !== null}
                        onClick={() => void addFriend(friend)}
                    >
                        {addingUserId === friend.userId
                            ? 'Adding...'
                            : 'Add friend'}
                    </button>
                )}
            </div>
        )
    }

    return (
        <div className="friends-page">
            <ChatHeader
                username={user?.username ?? 'User'}
                onLogout={() => {
                    void logout()
                }}
            />

            <main className="friends-page__content">
                <div className="friends-page__heading">
                    <p className="friends-page__eyebrow">
                        YOUR PEOPLE
                    </p>

                    <h1>Friends</h1>

                    <p className="friends-page__description">
                        Find people and build your circle.
                    </p>
                </div>

                {error && (
                    <div className="friends-page__message friends-page__message--error">
                        {error}
                    </div>
                )}

                {notice && (
                    <div className="friends-page__message friends-page__message--success">
                        {notice}
                    </div>
                )}

                <div className="friends-page__layout">
                    <section className="friends-page__card">
                        <div className="friends-page__card-heading">
                            <div>
                                <h2>Find people</h2>
                                <p>Search by username</p>
                            </div>
                        </div>

                        <label
                            className="friends-page__search-label"
                            htmlFor="friend-search"
                        >
                            Username
                        </label>

                        <div className="friends-page__search">
                            <span aria-hidden="true">⌕</span>
                            <input
                                id="friend-search"
                                type="search"
                                value={query}
                                onChange={event => setQuery(event.target.value)}
                                placeholder="Enter a username..."
                                autoComplete="off"
                            />
                            {query && (
                                <button
                                    type="button"
                                    className="friends-page__clear"
                                    onClick={() => setQuery('')}
                                    aria-label="Clear search"
                                >
                                    ×
                                </button>
                            )}
                        </div>

                        <div className="friends-page__results">
                            {!query.trim() ? (
                                <p className="friends-page__empty">
                                    Type a username to find people.
                                </p>
                            ) : isSearching ? (
                                <p className="friends-page__empty">
                                    Searching...
                                </p>
                            ) : results.length === 0 ? (
                                <p className="friends-page__empty">
                                    No users found. Try another username.
                                </p>
                            ) : (
                                results.map(renderSearchResult)
                            )}
                        </div>
                    </section>

                    <section className="friends-page__card">
                        <div className="friends-page__card-heading">
                            <div>
                                <h2>Your friends</h2>
                                <p>
                                    {friends.length}{' '}
                                    {friends.length === 1 ? 'friend' : 'friends'}
                                </p>
                            </div>
                        </div>

                        {isLoadingFriends ? (
                            <p className="friends-page__empty">
                                Loading your friends...
                            </p>
                        ) : friends.length === 0 ? (
                            <div className="friends-page__empty-block">
                                <div className="friends-page__empty-icon">
                                    ♡
                                </div>
                                <p>No friends yet</p>
                                <span>
                                    Search for someone and add them to get started.
                                </span>
                            </div>
                        ) : (
                            <div className="friends-page__list">
                                {friends.map(friend => (
                                    <div
                                        className="friends-page__person"
                                        key={friend.userId}
                                    >
                                        <div className="friends-page__person-avatar">
                                            {friend.username.charAt(0).toUpperCase()}
                                        </div>

                                        <div className="friends-page__person-info">
                                            <span className="friends-page__person-name">
                                                {friend.username}
                                            </span>
                                            <span className="friends-page__person-meta">
                                                Friend
                                            </span>
                                        </div>

                                        <span
                                            className="friends-page__friend-indicator"
                                            title="Friend"
                                            aria-label="Friend"
                                        >
                                            ✓
                                        </span>
                                    </div>
                                ))}
                            </div>
                        )}
                    </section>
                </div>
            </main>
        </div>
    )
}

export default FriendsPage