import type { ReactNode } from 'react'
import './ChatLayout.css'

interface ChatLayoutProps {
    header: ReactNode
    workspaces: ReactNode
    channels: ReactNode
    children: ReactNode
}

function ChatLayout({
    header,
    workspaces,
    channels,
    children,
}: ChatLayoutProps) {
    return (
        <div className="chat-app">
            {header}

            <div className="chat-layout">
                <aside className="chat-layout__workspaces">
                    {workspaces}
                </aside>

                <aside className="chat-layout__channels">
                    {channels}
                </aside>

                <main className="chat-layout__content">
                    {children}
                </main>
            </div>
        </div>
    )
}

export default ChatLayout