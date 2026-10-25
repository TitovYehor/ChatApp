import { useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { useGetOrCreatePrivateConversation } from '../features/privateConversations/usePrivateConversations'
import ChatHeader from '../layouts/ChatHeader'
import { useAuth } from '../features/auth/useAuth'
import { usePublicUserProfile } from '../features/users/usePublicUserProfile'
import { useProfileFriendship } from '../features/users/useProfileFriendship'
import { logError } from '../utils/logger'

import './css/UserProfilePage.css'

function UserProfilePage() {
    const { userId } = useParams<{ userId: string }>()
    const { user, logout } = useAuth()
    const navigate = useNavigate()

    const {
        profile,
        isLoading,
        error,
    } = usePublicUserProfile(userId)

    const [startConversationError, setStartConversationError] =
        useState<string | null>(null)

    const conversationMutation = useGetOrCreatePrivateConversation()

    const isOwnProfile = Boolean(userId && user?.id === userId)

    const {
        isFriend,
        isLoadingFriendship,
        isUpdatingFriendship,
        error: friendshipError,
        notice: friendshipNotice,
        toggleFriendship,
    } = useProfileFriendship(
        userId,
        profile?.username,
        isOwnProfile,
    )

    async function handleStartConversation() {
        if (!userId || !isFriend) {
            return
        }

        setStartConversationError(null)

        try {
            const conversation = await conversationMutation.mutateAsync(userId)
            navigate(`/conversations/${conversation.id}`)
        } catch (error) {
            logError('Failed to open private conversation', error)

            setStartConversationError(
                'Could not open this conversation. Please try again',
            )
        }
    }

    return (
        <div className="user-profile-page">
            <ChatHeader
                username={user?.username ?? 'User'}
                onLogout={() => {
                    void logout()
                }}
            />

            <main className="user-profile-page__content">
                <div className="user-profile-page__heading">
                    <p className="user-profile-page__eyebrow">
                        COMMUNITY
                    </p>

                    <h1>{isOwnProfile ? 'My profile' : 'User profile'}</h1>

                    <p className="user-profile-page__description">
                        {isOwnProfile
                            ? 'View your public profile.'
                            : 'Get to know someone in your community.'}
                    </p>
                </div>

                {isLoading ? (
                    <div className="user-profile-page__status">
                        Loading profile...
                    </div>
                ) : error || !profile ? (
                    <div className="user-profile-page__error">
                        {error ?? 'User not found.'}
                    </div>
                ) : (
                    <section className="user-profile-card">
                        <div className="user-profile-card__avatar">
                            {profile.username.charAt(0).toUpperCase()}
                        </div>

                        <h2 className="user-profile-card__username">
                            {profile.username}
                        </h2>

                        <p className="user-profile-card__label">
                            Member since
                        </p>

                        <p className="user-profile-card__date">
                            {new Date(profile.createdAt).toLocaleDateString(
                                undefined,
                                {
                                    year: 'numeric',
                                    month: 'long',
                                    day: 'numeric',
                                },
                            )}
                        </p>

                        {!isOwnProfile && (
                            <div className="user-profile-card__friend-actions">
                                <button
                                    type="button"
                                    className={
                                        isFriend
                                            ? 'user-profile-card__friend-button user-profile-card__friend-button--remove'
                                            : 'user-profile-card__friend-button'
                                    }
                                    disabled={
                                        isLoadingFriendship ||
                                        isUpdatingFriendship ||
                                        !userId
                                    }
                                    onClick={() => {
                                        void toggleFriendship()
                                    }}
                                >
                                    {isLoadingFriendship
                                        ? 'Checking...'
                                        : isUpdatingFriendship
                                            ? 'Please wait...'
                                            : isFriend
                                                ? 'Remove friend'
                                                : 'Add friend'}
                                </button>

                                {friendshipError && (
                                    <p className="user-profile-card__friend-error" role="alert">
                                        {friendshipError}
                                    </p>
                                )}

                                {friendshipNotice && (
                                    <p className="user-profile-card__friend-notice" role="status">
                                        {friendshipNotice}
                                    </p>
                                )}

                                <button
                                    type="button"
                                    className="user-profile-card__friend-button"
                                    disabled={
                                        !isFriend ||
                                        isLoadingFriendship ||
                                        conversationMutation.isPending
                                    }
                                    onClick={() => {
                                        void handleStartConversation()
                                    }}
                                >
                                    {conversationMutation.isPending
                                        ? 'Opening conversation...'
                                        : 'Message'}
                                </button>

                                {startConversationError && (
                                    <p className="user-profile-card__error" role="alert">
                                        {startConversationError}
                                    </p>
                                )}
                            </div>
                        )}

                        {isOwnProfile && (
                            <Link
                                to="/profile"
                                className="user-profile-card__settings"
                            >
                                Account settings
                            </Link>
                        )}
                    </section>
                )}

                <Link
                    to="/friends"
                    className="user-profile-page__back-link"
                >
                    ← Back to friends
                </Link>
            </main>
        </div>
    )
}

export default UserProfilePage