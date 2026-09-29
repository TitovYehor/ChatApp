import { useEffect, useRef, useState } from 'react'
import type {
    PointerEvent as ReactPointerEvent,
    ReactNode,
} from 'react'

import './ChatLayout.css'

interface ChatLayoutProps {
    header: ReactNode
    workspaces: ReactNode
    channels: ReactNode
    children: ReactNode
}

const MIN_WORKSPACE_WIDTH = 200
const MAX_WORKSPACE_WIDTH = 450

const MIN_CHANNEL_WIDTH = 200
const MAX_CHANNEL_WIDTH = 450

const MIN_MESSAGE_WIDTH = 320

function getSavedWidth(
    key: string,
    defaultWidth: number,
    minWidth: number,
    maxWidth: number,
): number {
    try {
        const savedWidth = localStorage.getItem(key)

        if (savedWidth === null) {
            return defaultWidth
        }

        const width = Number(savedWidth)

        if (!Number.isFinite(width)) {
            return defaultWidth
        }

        return Math.max(
            minWidth,
            Math.min(width, maxWidth),
        )
    } catch (error) {
        console.error(
            `Failed to load saved width for "${key}":`,
            error,
        )

        return defaultWidth
    }
}

interface ResizeState {
    divider: 'workspaces' | 'channels'
    startX: number
    startWorkspaceWidth: number
    startChannelWidth: number
}

function ChatLayout({
    header,
    workspaces,
    channels,
    children,
}: ChatLayoutProps) {
    const [workspaceWidth, setWorkspaceWidth] = useState(() =>
        getSavedWidth(
            'chat-workspace-width',
            280,
            MIN_WORKSPACE_WIDTH,
            MAX_WORKSPACE_WIDTH,
        ),
    )

    const [channelWidth, setChannelWidth] = useState(() =>
        getSavedWidth(
            'chat-channel-width',
            260,
            MIN_CHANNEL_WIDTH,
            MAX_CHANNEL_WIDTH,
        ),
    )

    useEffect(() => {
        try {
            localStorage.setItem(
                'chat-workspace-width',
                String(workspaceWidth),
            )
        } catch (error) {
            console.error(
                'Failed to save workspace panel width:',
                error,
            )
        }
    }, [workspaceWidth])

    useEffect(() => {
        try {
            localStorage.setItem(
                'chat-channel-width',
                String(channelWidth),
            )
        } catch (error) {
            console.error(
                'Failed to save channel panel width:',
                error,
            )
        }
    }, [channelWidth])

    const layoutRef = useRef<HTMLDivElement>(null)
    const resizeRef = useRef<ResizeState | null>(null)

    function handlePointerDown(
        event: ReactPointerEvent<HTMLDivElement>,
        divider: 'workspaces' | 'channels',
    ) {
        event.preventDefault()

        resizeRef.current = {
            divider,
            startX: event.clientX,
            startWorkspaceWidth: workspaceWidth,
            startChannelWidth: channelWidth,
        }

        event.currentTarget.setPointerCapture(event.pointerId)
    }

    function handlePointerMove(
        event: ReactPointerEvent<HTMLDivElement>,
    ) {
        const resize = resizeRef.current
        const layout = layoutRef.current

        if (!resize || !layout) {
            return
        }

        const delta = event.clientX - resize.startX
        const availableWidth = layout.clientWidth

        if (resize.divider === 'workspaces') {
            const maxAllowedWidth = Math.min(
                MAX_WORKSPACE_WIDTH,
                availableWidth -
                    MIN_CHANNEL_WIDTH -
                    MIN_MESSAGE_WIDTH,
            )

            const nextWidth = Math.max(
                MIN_WORKSPACE_WIDTH,
                Math.min(
                    resize.startWorkspaceWidth + delta,
                    maxAllowedWidth,
                ),
            )

            setWorkspaceWidth(nextWidth)
        } else {
            const maxAllowedWidth = Math.min(
                MAX_CHANNEL_WIDTH,
                availableWidth -
                    workspaceWidth -
                    MIN_MESSAGE_WIDTH,
            )

            const nextWidth = Math.max(
                MIN_CHANNEL_WIDTH,
                Math.min(
                    resize.startChannelWidth + delta,
                    maxAllowedWidth,
                ),
            )

            setChannelWidth(nextWidth)
        }
    }

    function handlePointerUp(
        event: ReactPointerEvent<HTMLDivElement>,
    ) {
        resizeRef.current = null

        if (
            event.currentTarget.hasPointerCapture(
                event.pointerId,
            )
        ) {
            event.currentTarget.releasePointerCapture(
                event.pointerId,
            )
        }
    }

    function handlePointerCancel() {
        resizeRef.current = null
    }

    return (
        <div className="chat-app">
            {header}

            <div
                ref={layoutRef}
                className="chat-layout"
                style={{
                    gridTemplateColumns: `
                        ${workspaceWidth}px
                        6px
                        ${channelWidth}px
                        6px
                        minmax(0, 1fr)
                    `,
                }}
            >
                <aside className="chat-layout__workspaces">
                    {workspaces}
                </aside>

                <div
                    className="chat-layout__resizer"
                    role="separator"
                    aria-label="Resize workspaces panel"
                    aria-orientation="vertical"
                    onPointerDown={(event) =>
                        handlePointerDown(event, 'workspaces')
                    }
                    onPointerMove={handlePointerMove}
                    onPointerUp={handlePointerUp}
                    onPointerCancel={handlePointerCancel}
                />

                <aside className="chat-layout__channels">
                    {channels}
                </aside>

                <div
                    className="chat-layout__resizer"
                    role="separator"
                    aria-label="Resize channels panel"
                    aria-orientation="vertical"
                    onPointerDown={(event) =>
                        handlePointerDown(event, 'channels')
                    }
                    onPointerMove={handlePointerMove}
                    onPointerUp={handlePointerUp}
                    onPointerCancel={handlePointerCancel}
                />

                <main className="chat-layout__content">
                    {children}
                </main>
            </div>
        </div>
    )
}

export default ChatLayout