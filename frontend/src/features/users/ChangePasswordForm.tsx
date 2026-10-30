import {
    useState,
} from 'react'

import './css/ChangePasswordForm.css'

interface ChangePasswordFormProps {
    isDemoAccount: boolean
    isChangingPassword: boolean
    changePasswordError: string | null
    onChangePassword: (
        currentPassword: string,
        newPassword: string,
    ) => Promise<void>
}

function ChangePasswordForm({
    isDemoAccount,
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

        if (isDemoAccount) {
            return
        }

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
        <section className="password-card">
            <div className="password-card__heading">
                <div className="password-card__icon">
                    *
                </div>

                <div>
                    <h2>Security</h2>
                    <p>Change your password to keep your account secure</p>
                </div>
            </div>

            <form
                className="password-form"
                onSubmit={handleSubmit}
            >
                <div className="password-form__field">
                    <label htmlFor="current-password">
                        Current password
                    </label>

                    <input
                        id="current-password"
                        type="password"
                        autoComplete="current-password"
                        value={currentPassword}
                        onChange={(event) =>
                            setCurrentPassword(event.target.value)
                        }
                        disabled={isChangingPassword || isDemoAccount}
                        required
                    />
                </div>

                <div className="password-form__field">
                    <label htmlFor="new-password">
                        New password
                    </label>

                    <input
                        id="new-password"
                        type="password"
                        autoComplete="new-password"
                        value={newPassword}
                        onChange={(event) =>
                            setNewPassword(event.target.value)
                        }
                        disabled={isChangingPassword || isDemoAccount}
                        required
                    />

                    <span className="password-form__hint">
                        Use at least 6 characters
                    </span>
                </div>

                <div className="password-form__field">
                    <label htmlFor="confirm-password">
                        Confirm new password
                    </label>

                    <input
                        id="confirm-password"
                        type="password"
                        autoComplete="new-password"
                        value={confirmPassword}
                        onChange={(event) =>
                            setConfirmPassword(event.target.value)
                        }
                        disabled={isChangingPassword || isDemoAccount}
                        required
                    />
                </div>

                {validationError && (
                    <p className="password-form__error" role="alert">
                        {validationError}
                    </p>
                )}

                {changePasswordError && (
                    <p className="password-form__error" role="alert">
                        {changePasswordError}
                    </p>
                )}

                <button
                    type="submit"
                    className="password-form__submit"
                    disabled={isChangingPassword || isDemoAccount}
                >
                    {isChangingPassword
                        ? 'Updating password...'
                        : 'Update password'}
                </button>
            </form>
        </section>
    )
}

export default ChangePasswordForm