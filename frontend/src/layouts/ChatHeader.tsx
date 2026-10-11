import { Link, useLocation } from 'react-router-dom'
import './ChatHeader.css'

interface ChatHeaderProps {
    username: string
    onLogout: () => void
}

function ChatHeader({
    username,
    onLogout,
}: ChatHeaderProps) {
    const location = useLocation()
    const isProfilePage = location.pathname === '/profile'

    return (
        <header className="chat-header">
            <Link
                to="/chat"
                className="chat-header__brand"
                aria-label="Chat App home"
            >
                <div className="chat-header__logo">
                    C
                </div>

                <span className="chat-header__title">
                    Chat App
                </span>
            </Link>

            <div className="chat-header__user">
                <Link
                    to="/profile"
                    className={`chat-header__profile-link${
                        isProfilePage
                            ? ' chat-header__profile-link--active'
                            : ''
                    }`}
                    aria-current={isProfilePage ? 'page' : undefined}
                >
                    Profile
                </Link>

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