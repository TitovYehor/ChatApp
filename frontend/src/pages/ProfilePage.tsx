import ChatHeader from '../layouts/ChatHeader'
import ProfileView from '../features/users/ProfileView'
import ChangePasswordForm from '../features/users/ChangePasswordForm'
import { useUserProfile } from '../features/users/useUserProfile'
import { useAuth } from '../features/auth/useAuth'

import './css/ProfilePage.css'

function ProfilePage() {
    const {
        user,
        logout,
    } = useAuth()

    const {
        profile,
        isLoading,
        error,
        updateProfile,
        isUpdatingProfile,
        updateProfileError,
        changePassword,
        isChangingPassword,
        changePasswordError,
    } = useUserProfile()

    async function handleUpdateUsername(username: string) {
        await updateProfile({ username })
    }

    async function handleChangePassword(
        currentPassword: string,
        newPassword: string,
    ) {
        await changePassword({
            currentPassword,
            newPassword,
        })
    }

    return (
        <div className="profile-page">
            <ChatHeader
                username={user?.username ?? 'User'}
                onLogout={() => {
                    void logout()
                }}
            />

            <main className="profile-page__content">
                <div className="profile-page__heading">
                    <p className="profile-page__eyebrow">
                        ACCOUNT SETTINGS
                    </p>

                    <h1>My profile</h1>

                    <p className="profile-page__description">
                        Manage your account details and security.
                    </p>
                </div>

                {isLoading ? (
                    <div className="profile-page__status">
                        Loading your profile...
                    </div>
                ) : error || !profile ? (
                    <div className="profile-page__error">
                        {error ?? 'Profile not found'}
                    </div>
                ) : (
                    <div className="profile-page__sections">
                        <ProfileView
                            profile={profile}
                            isUpdating={isUpdatingProfile}
                            updateError={updateProfileError}
                            onUpdateUsername={handleUpdateUsername}
                        />

                        <ChangePasswordForm
                            isChangingPassword={isChangingPassword}
                            changePasswordError={changePasswordError}
                            onChangePassword={handleChangePassword}
                        />
                    </div>
                )}
            </main>
        </div>
    )
}

export default ProfilePage