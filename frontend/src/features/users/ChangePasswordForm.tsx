import {
    useState,
} from 'react'

interface ChangePasswordFormProps {
    isChangingPassword: boolean
    changePasswordError: string | null
    onChangePassword: (
        currentPassword: string,
        newPassword: string,
    ) => Promise<void>
}

function ChangePasswordForm({
    isChangingPassword,
    changePasswordError,
    onChangePassword,
}: ChangePasswordFormProps) {
    const [
        currentPassword,
        setCurrentPassword,
    ] = useState('')

    const [
        newPassword,
        setNewPassword,
    ] = useState('')

    const [
        confirmPassword,
        setConfirmPassword,
    ] = useState('')

    const [
        validationError,
        setValidationError,
    ] = useState<string | null>(
        null,
    )

    async function handleSubmit(
        event: React.FormEvent<HTMLFormElement>,
    ) {
        event.preventDefault()

        setValidationError(
            null,
        )

        if (
            !currentPassword ||
            !newPassword ||
            !confirmPassword
        ) {
            setValidationError(
                'All password fields are required',
            )

            return
        }

        if (
            newPassword !==
            confirmPassword
        ) {
            setValidationError(
                'New passwords do not match',
            )

            return
        }

        if (
            newPassword.length < 6
        ) {
            setValidationError(
                'New password must be at least 6 characters',
            )

            return
        }

        await onChangePassword(
            currentPassword,
            newPassword,
        )

        setCurrentPassword('')
        setNewPassword('')
        setConfirmPassword('')
    }

    return (
        <section>
            <h2>
                Change password
            </h2>

            <form
                onSubmit={
                    handleSubmit
                }
            >
                <div>
                    <label htmlFor="current-password">
                        Current password
                    </label>

                    <input
                        id="current-password"
                        type="password"
                        value={
                            currentPassword
                        }
                        onChange={(
                            event,
                        ) =>
                            setCurrentPassword(
                                event.target
                                    .value,
                            )
                        }
                        disabled={
                            isChangingPassword
                        }
                    />
                </div>

                <div>
                    <label htmlFor="new-password">
                        New password
                    </label>

                    <input
                        id="new-password"
                        type="password"
                        value={
                            newPassword
                        }
                        onChange={(
                            event,
                        ) =>
                            setNewPassword(
                                event.target
                                    .value,
                            )
                        }
                        disabled={
                            isChangingPassword
                        }
                    />
                </div>

                <div>
                    <label htmlFor="confirm-password">
                        Confirm new password
                    </label>

                    <input
                        id="confirm-password"
                        type="password"
                        value={
                            confirmPassword
                        }
                        onChange={(
                            event,
                        ) =>
                            setConfirmPassword(
                                event.target
                                    .value,
                            )
                        }
                        disabled={
                            isChangingPassword
                        }
                    />
                </div>

                {validationError && (
                    <p>
                        {validationError}
                    </p>
                )}

                {changePasswordError && (
                    <p>
                        {changePasswordError}
                    </p>
                )}

                <button
                    type="submit"
                    disabled={
                        isChangingPassword
                    }
                >
                    {isChangingPassword
                        ? 'Changing...'
                        : 'Change password'}
                </button>
            </form>
        </section>
    )
}

export default ChangePasswordForm