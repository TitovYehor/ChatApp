import type {
    PrivateUserTypingResponse,
} from '../../types/presenceTypes'

import "../css/TypingIndicator.css"

interface PrivateTypingIndicatorProps {
    typingUsers: PrivateUserTypingResponse[]
}

export default function PrivateTypingIndicator({
    typingUsers,
}: PrivateTypingIndicatorProps) {
    if (typingUsers.length === 0) {
        return null
    }

    const text = typingUsers.length === 1
        ? `${typingUsers[0].username} is typing…`
        : `${typingUsers.map((user) => user.username).join(', ')} are typing…`

    return (
        <p className="private-typing-indicator" aria-live="polite">
            {text}
        </p>
    )
}