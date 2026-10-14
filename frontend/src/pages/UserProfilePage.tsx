import { Link, useParams } from 'react-router-dom'

import ChatHeader from '../layouts/ChatHeader'
import { useAuth } from '../features/auth/useAuth'
import { usePublicUserProfile } from '../features/users/usePublicUserProfile'

import './css/UserProfilePage.css'

function UserProfilePage() {
    const { userId } = useParams<{ userId: string }>()
    const { user, logout } = useAuth()

    const {
        profile,
        isLoading,
        error,
    } = usePublicUserProfile(userId)

    const isOwnProfile = Boolean(userId && user?.id === userId)

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