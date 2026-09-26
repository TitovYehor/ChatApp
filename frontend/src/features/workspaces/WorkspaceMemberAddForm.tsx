import {
    useState,
} from 'react'

import './css/WorkspaceMemberAddForm.css'

interface WorkspaceMemberAddFormProps {
    isAdding: boolean
    addError: string | null

    onAdd: (
        usernameOrEmail: string,
    ) => Promise<void>
}

function WorkspaceMemberAddForm({
    isAdding,
    addError,
    onAdd,
}: WorkspaceMemberAddFormProps) {
    const [
        usernameOrEmail,
        setUsernameOrEmail,
    ] = useState('')

    async function handleSubmit(
        event: React.SubmitEvent,
    ) {
        event.preventDefault()

        const value =
            usernameOrEmail.trim()

        if (!value) {
            return
        }

        await onAdd(
            value,
        )

        setUsernameOrEmail('')
    }

    return (
        <form
            className="workspace-member-add-form"
            onSubmit={
                handleSubmit
            }
        >
            <h4 className="workspace-member-add-form__title">
                Add member
            </h4>

            <div className="workspace-member-add-form__row">
                <input
                    className="workspace-member-add-form__input"
                    type="text"
                    value={
                        usernameOrEmail
                    }
                    onChange={(
                        event,
                    ) =>
                        setUsernameOrEmail(
                            event.target
                                .value,
                        )
                    }
                    placeholder="Username or email..."
                    disabled={
                        isAdding
                    }
                />

                <button
                    className="workspace-member-add-form__button"
                    type="submit"
                    disabled={
                        isAdding ||
                        usernameOrEmail
                            .trim()
                            .length === 0
                    }
                >
                    {isAdding
                        ? 'Adding...'
                        : 'Add member'}
                </button>
            </div>

            {addError && (
                <p className="workspace-member-add-form__error">
                    {addError}
                </p>
            )}
        </form>
    )
}

export default WorkspaceMemberAddForm