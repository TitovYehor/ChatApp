import {
    useEffect,
    useRef,
    useState,
} from 'react'

interface MessageComposerProps {
    isSending: boolean
    sendError: string | null

    onSend: (
        content: string,
    ) => Promise<void>

    onStartTyping: () => Promise<void>
    onStopTyping: () => Promise<void>
}

function MessageComposer({
    isSending,
    sendError,
    onSend,
    onStartTyping,
    onStopTyping,
}: MessageComposerProps) {
    const [
        content,
        setContent,
    ] = useState('')

    const typingTimeoutRef =
        useRef<ReturnType<
            typeof setTimeout
        > | null>(null)

    useEffect(() => {
        return () => {
            if (
                typingTimeoutRef.current
            ) {
                clearTimeout(
                    typingTimeoutRef.current,
                )
            }

            void onStopTyping()
        }
    }, [
        onStopTyping,
    ])

    function handleChange(
        value: string,
    ) {
        setContent(value)

        if (!value.trim()) {
            void onStopTyping()
            return
        }

        void onStartTyping()

        if (
            typingTimeoutRef.current
        ) {
            clearTimeout(
                typingTimeoutRef.current,
            )
        }

        typingTimeoutRef.current =
            setTimeout(() => {
                void onStopTyping()
                typingTimeoutRef.current =
                    null
            }, 1500)
    }

    async function handleSubmit(
        event: React.SubmitEvent,
    ) {
        event.preventDefault()

        const trimmedContent =
            content.trim()

        if (!trimmedContent) {
            return
        }

        if (
            typingTimeoutRef.current
        ) {
            clearTimeout(
                typingTimeoutRef.current,
            )

            typingTimeoutRef.current =
                null
        }

        await onStopTyping()

        await onSend(
            trimmedContent,
        )

        setContent('')
    }

    return (
        <form
            onSubmit={
                handleSubmit
            }
        >
            <input
                type="text"
                value={content}
                onChange={(
                    event,
                ) =>
                    handleChange(
                        event.target
                            .value,
                    )
                }
                placeholder="Write a message..."
                disabled={
                    isSending
                }
            />

            <button
                type="submit"
                disabled={
                    isSending ||
                    content
                        .trim()
                        .length ===
                    0
                }
            >
                {isSending
                    ? 'Sending...'
                    : 'Send'}
            </button>

            {sendError && (
                <p>
                    {sendError}
                </p>
            )}
        </form>
    )
}

export default MessageComposer