import {
    useState,
} from 'react'

import './css/ChannelCreateForm.css'

interface ChannelCreateFormProps {
    isCreating: boolean
    createError: string | null

    onCreate: (
        name: string,
    ) => Promise<void>
}

function ChannelCreateForm({
    isCreating,
    createError,
    onCreate,
}: ChannelCreateFormProps) {
    const [
        name,
        setName,
    ] = useState('')

    async function handleSubmit(
        event: React.SubmitEvent,
    ) {
        event.preventDefault()

        const trimmedName =
            name.trim()

        if (!trimmedName) {
            return
        }

        await onCreate(
            trimmedName,
        )

        setName('')
    }

    return (
        <form
            className="channel-create-form"
            onSubmit={
                handleSubmit
            }
        >
            <input
                className="channel-create-form__input"
                type="text"
                value={name}
                onChange={(
                    event,
                ) =>
                    setName(
                        event.target.value,
                    )
                }
                placeholder="Channel name..."
                maxLength={100}
                disabled={
                    isCreating
                }
            />

            <button
                className="channel-create-form__button"
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
                    : 'Create channel'}
            </button>

            {createError && (
                <p className="channel-create-form__error">
                    {createError}
                </p>
            )}
        </form>
    )
}

export default ChannelCreateForm