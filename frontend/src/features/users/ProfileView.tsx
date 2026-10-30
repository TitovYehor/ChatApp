import type {
    UserProfileResponse,
} from '../../types/userTypes'

import './css/ProfileView.css'

interface ProfileViewProps {
    profile: UserProfileResponse
    isDemoAccount: boolean
    isUpdating: boolean
    updateError: string | null
    onUpdateUsername: (
        username: string,
    ) => Promise<void>
}

function ProfileView({
    profile,
    isDemoAccount,
    isUpdating,
    updateError,
    onUpdateUsername,
}: ProfileViewProps) {
    async function handleSubmit(
        event: React.FormEvent<HTMLFormElement>,
    ) {
        event.preventDefault()

        if (isDemoAccount) {
            return
        }

        const formData =
            new FormData(
                event.currentTarget,
            )

        const username =
            String(
                formData.get(
                    'username',
                ) ?? '',
            ).trim()

        if (!username) {
            return
        }

        await onUpdateUsername(
            username,
        )
    }

    
    return (
        <section className="profile-card">
            <div className="profile-card__heading">
                <div className="profile-card__icon">
                    @
                </div>

                <div>
                    <h2>Personal information</h2>
                    <p>Update the details associated with your account</p>
                </div>
            </div>

            <div className="profile-card__details">
                <div className="profile-card__detail">
                    <span>Email address</span>
                    <strong>{profile.email}</strong>
                </div>

                <div className="profile-card__detail">
                    <span>Member since</span>
                    <strong>
                        {new Date(profile.createdAt).toLocaleDateString(
                            undefined,
                            {
                                day: 'numeric',
                                month: 'long',
                                year: 'numeric',
                            },
                        )}
                    </strong>
                </div>
            </div>

            <form className="profile-form" onSubmit={handleSubmit}>
                <div className="profile-form__field">
                    <label htmlFor="username">Username</label>

                    <input
                        id="username"
                        name="username"
                        type="text"
                        defaultValue={profile.username}
                        maxLength={50}
                        autoComplete="username"
                        disabled={isUpdating || isDemoAccount}
                        required
                    />
                </div>

                {updateError && (
                    <p className="profile-form__error" role="alert">
                        {updateError}
                    </p>
                )}

                <button
                    type="submit"
                    className="profile-form__submit"
                    disabled={isUpdating || isDemoAccount}
                >
                    {isUpdating ? 'Saving changes...' : 'Save changes'}
                </button>
            </form>
        </section>
    )
}

export default ProfileView