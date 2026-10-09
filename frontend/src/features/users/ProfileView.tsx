import type {
    UserProfileResponse,
} from '../../types/userTypes'

interface ProfileViewProps {
    profile: UserProfileResponse
    isUpdating: boolean
    updateError: string | null
    onUpdateUsername: (
        username: string,
    ) => Promise<void>
}

function ProfileView({
    profile,
    isUpdating,
    updateError,
    onUpdateUsername,
}: ProfileViewProps) {
    async function handleSubmit(
        event: React.FormEvent<HTMLFormElement>,
    ) {
        event.preventDefault()

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
        <section>
            <h1>Profile</h1>

            <p>
                <strong>Email:</strong>{' '}
                {profile.email}
            </p>

            <p>
                <strong>Account created:</strong>{' '}
                {new Date(
                    profile.createdAt,
                ).toLocaleDateString()}
            </p>

            <form
                onSubmit={
                    handleSubmit
                }
            >
                <div>
                    <label htmlFor="username">
                        Username
                    </label>

                    <input
                        id="username"
                        name="username"
                        type="text"
                        defaultValue={
                            profile.username
                        }
                        maxLength={50}
                        disabled={
                            isUpdating
                        }
                    />
                </div>

                <button
                    type="submit"
                    disabled={
                        isUpdating
                    }
                >
                    {isUpdating
                        ? 'Saving...'
                        : 'Save username'}
                </button>
            </form>

            {updateError && (
                <p>
                    {updateError}
                </p>
            )}
        </section>
    )
}

export default ProfileView