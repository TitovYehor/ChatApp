import './ChatHeader.css'

interface ChatHeaderProps {
    username: string
    onLogout: () => void
}

function ChatHeader({
    username,
    onLogout,
}: ChatHeaderProps) {
    return (
        <header className="chat-header">
            <div className="chat-header__brand">
                <div className="chat-header__logo">
                    C
                </div>

                <span className="chat-header__title">
                    Chat App
                </span>
            </div>

            <div className="chat-header__user">
                <div className="chat-header__avatar">
                    {username.charAt(0).toUpperCase()}
                </div>

                <span className="chat-header__username">
                    {username}
                </span>

                <button
                    type="button"
                    className="chat-header__logout"
                    onClick={onLogout}
                >
                    Logout
                </button>
            </div>
        </header>
    )
}

export default ChatHeader