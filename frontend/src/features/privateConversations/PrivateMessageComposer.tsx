import { useEffect, useRef, useState } from 'react'

import './css/PrivateMessageComposer.css'

interface PrivateMessageComposerProps {
    isSending: boolean
    sendError: string | null
    onSend: (content: string) => Promise<void>
    onTypingStart: () => Promise<void>
    onTypingStop: () => Promise<void>
}

const TYPING_STOP_DELAY = 1500

export default function PrivateMessageComposer({
    isSending,
    sendError,
    onSend,
    onTypingStart,
    onTypingStop,
}: PrivateMessageComposerProps) {
    const [content, setContent] = useState('')
    const [typingError, setTypingError] = useState<string | null>(null)

    const typingActive = useRef(false)
    const typingTimeout = useRef<ReturnType<typeof setTimeout> | null>(null)

    async function stopTyping() {
        if (typingTimeout.current) {
            clearTimeout(typingTimeout.current)
            typingTimeout.current = null
        }

        if (!typingActive.current) {
            return
        }

        typingActive.current = false

        try {
            await onTypingStop()
        } catch {
            setTypingError('Could not update your typing status.')
        }
    }

    async function startTyping() {
        if (typingActive.current) {
            return
        }

        typingActive.current = true

        try {
            await onTypingStart()
            setTypingError(null)
        } catch {
            typingActive.current = false
            setTypingError('Could not update your typing status.')
        }
    }

    function handleChange(value: string) {
        setContent(value)

        if (!value.trim()) {
            void stopTyping()
            return
        }

        void startTyping()

        if (typingTimeout.current) {
            clearTimeout(typingTimeout.current)
        }

        typingTimeout.current = setTimeout(() => {
            void stopTyping()
        }, TYPING_STOP_DELAY)
    }

    async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault()

        const trimmedContent = content.trim()

        if (!trimmedContent || isSending) {
            return
        }

        await stopTyping()
        await onSend(trimmedContent)
        setContent('')
    }

    useEffect(() => {
        return () => {
            if (typingTimeout.current) {
                clearTimeout(typingTimeout.current)
            }

            if (typingActive.current) {
                typingActive.current = false
                void onTypingStop().catch(() => { })
            }
        }
    }, [onTypingStop])

    return (
        <form className="private-message-composer" onSubmit={handleSubmit}>
            <textarea
                value={content}
                onChange={(event) => handleChange(event.target.value)}
                placeholder="Write a message..."
                aria-label="Write a private message"
                rows={2}
                disabled={isSending}
                onKeyDown={(event) => {
                    if (event.key === 'Enter' && !event.shiftKey) {
                        event.preventDefault()
                        event.currentTarget.form?.requestSubmit()
                    }
                }}
            />

            <div className="private-message-composer__footer">
                <div aria-live="polite">
                    {sendError && (
                        <p className="private-message-composer__error">
                            {sendError}
                        </p>
                    )}

                    {typingError && (
                        <p className="private-message-composer__error">
                            {typingError}
                        </p>
                    )}
                </div>

                <button
                    type="submit"
                    disabled={isSending || !content.trim()}
                >
                    {isSending ? 'Sending…' : 'Send'}
                </button>
            </div>
        </form>
    )
}