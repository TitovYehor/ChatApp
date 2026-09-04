import {
    useState,
} from 'react'

interface WorkspaceCreateFormProps {
    isCreating: boolean
    createError: string | null

    onCreate: (
        name: string,
        description: string,
    ) => Promise<void>
}

function WorkspaceCreateForm({
    isCreating,
    createError,
    onCreate,
}: WorkspaceCreateFormProps) {
    const [
        name,
        setName,
    ] = useState('')

    const [
        description,
        setDescription,
    ] = useState('')

    async function handleSubmit(
        event: React.SubmitEvent,
    ) {
        event.preventDefault()

        const trimmedName =
            name.trim()

        const trimmedDescription =
            description.trim()

        if (!trimmedName) {
            return
        }

        await onCreate(
            trimmedName,
            trimmedDescription,
        )

        setName('')
        setDescription('')
    }

    return (
        <form
            onSubmit={
                handleSubmit
            }
        >
            <h3>
                Create workspace
            </h3>

            <input
                type="text"
                value={name}
                onChange={(
                    event,
                ) =>
                    setName(
                        event.target.value,
                    )
                }
                placeholder="Workspace name..."
                maxLength={100}
                disabled={
                    isCreating
                }
            />

            <textarea
                value={
                    description
                }
                onChange={(
                    event,
                ) =>
                    setDescription(
                        event.target.value,
                    )
                }
                placeholder="Description..."
                disabled={
                    isCreating
                }
            />

            <button
                type="submit"
                disabled={
                    isCreating ||
                    name.trim()
                        .length ===
                    0
                }
            >
                {isCreating
                    ? 'Creating...'
                    : 'Create workspace'}
            </button>

            {createError && (
                <p>
                    {createError}
                </p>
            )}
        </form>
    )
}

export default WorkspaceCreateForm