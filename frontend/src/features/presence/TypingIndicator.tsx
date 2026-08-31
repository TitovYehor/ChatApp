import type {
    UserTypingResponse,
} from '../../types/presenceTypes'

interface TypingIndicatorProps {
    typingUsers: UserTypingResponse[]
}

function TypingIndicator({
    typingUsers,
}: TypingIndicatorProps) {
    if (typingUsers.length === 0) {
        return null
    }

    if (typingUsers.length === 1) {
        return (
            <p>
                {
                    typingUsers[0]
                        .username
                }{' '}
                is typing...
            </p>
        )
    }

    if (typingUsers.length === 2) {
        return (
            <p>
                {
                    typingUsers[0]
                        .username
                }{' '}
                and{' '}
                {
                    typingUsers[1]
                        .username
                }{' '}
                are typing...
            </p>
        )
    }

    return (
        <p>
            {
                typingUsers[0]
                    .username
            }{' '}
            and{' '}
            {typingUsers.length -
                1}{' '}
            others are typing...
        </p>
    )
}

export default TypingIndicator