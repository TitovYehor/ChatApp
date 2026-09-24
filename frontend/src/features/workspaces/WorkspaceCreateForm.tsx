import {
    useState,
} from 'react'

import './css/WorkspaceCreateForm.css'

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
            className="workspace-create-form"
            onSubmit={handleSubmit}
        >
            <h3 className="workspace-create-form__title">
                Create workspace
            </h3>

            <input
                className="workspace-create-form__input"
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
                className="workspace-create-form__textarea"
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
                className="workspace-create-form__button"
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
                <p className="workspace-create-form__error">
                    {createError}
                </p>
            )}
        </form>
    )
}

export default WorkspaceCreateForm