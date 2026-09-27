import {
    useEffect,
    useRef,
    useState,
} from 'react'

import type {
    FormEvent,
    KeyboardEvent,
} from 'react'

import './css/MessageComposer.css'

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
    const [content, setContent] = useState('')

    const formRef = useRef<HTMLFormElement>(null)

    const typingTimeoutRef =
        useRef<ReturnType<typeof setTimeout> | null>(null)

    useEffect(() => {
        return () => {
            if (typingTimeoutRef.current) {
                clearTimeout(typingTimeoutRef.current)
            }

            void onStopTyping()
        }
    }, [onStopTyping])

    function clearTypingTimeout() {
        if (typingTimeoutRef.current) {
            clearTimeout(typingTimeoutRef.current)
            typingTimeoutRef.current = null
        }
    }

    function handleChange(value: string) {
        setContent(value)

        if (!value.trim()) {
            clearTypingTimeout()
            void onStopTyping()
            return
        }

        void onStartTyping()

        clearTypingTimeout()

        typingTimeoutRef.current = setTimeout(() => {
            void onStopTyping()
            typingTimeoutRef.current = null
        }, 1500)
    }

    function handleKeyDown(
        event: KeyboardEvent<HTMLTextAreaElement>,
    ) {
        if (
            event.key === 'Enter' &&
            !event.shiftKey &&
            !event.nativeEvent.isComposing
        ) {
            event.preventDefault()

            if (!isSending && content.trim()) {
                formRef.current?.requestSubmit()
            }
        }
    }

    async function handleSubmit(
        event: FormEvent<HTMLFormElement>,
    ) {
        event.preventDefault()

        const trimmedContent = content.trim()

        if (!trimmedContent || isSending) {
            return
        }

        clearTypingTimeout()

        await onStopTyping()

        await onSend(trimmedContent)

        setContent('')
    }

    return (
        <form
            ref={formRef}
            className="message-composer"
            onSubmit={handleSubmit}
        >
            <div className="message-composer__container">
                <textarea
                    className="message-composer__input"
                    value={content}
                    onChange={(event) => {
                        handleChange(event.target.value)
                    }}
                    onKeyDown={handleKeyDown}
                    placeholder="Write a message..."
                    rows={1}
                    disabled={isSending}
                />

                <button
                    type="submit"
                    className="message-composer__button"
                    disabled={
                        isSending ||
                        content.trim().length === 0
                    }
                >
                    {isSending ? 'Sending...' : 'Send'}
                </button>
            </div>

            {sendError && (
                <p className="message-composer__error">
                    {sendError}
                </p>
            )}

            <p className="message-composer__hint">
                Enter to send · Shift + Enter for a new line
            </p>
        </form>
    )
}

export default MessageComposer