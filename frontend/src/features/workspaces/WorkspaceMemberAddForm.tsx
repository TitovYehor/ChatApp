import {
    useState,
} from 'react'

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
            onSubmit={
                handleSubmit
            }
        >
            <h4>
                Add member
            </h4>

            <input
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

            {addError && (
                <p>
                    {addError}
                </p>
            )}
        </form>
    )
}

export default WorkspaceMemberAddForm