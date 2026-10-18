import { useState } from 'react'
import './css/NotificationCenter.css'

import { useNotifications } from './useNotifications'
import type { NotificationResponse } from '../../types/notificationTypes'

function formatNotificationTime(value: string): string {
    const date = new Date(value)

    if (Number.isNaN(date.getTime())) {
        return ''
    }

    return new Intl.DateTimeFormat(undefined, {
        dateStyle: 'short',
        timeStyle: 'short',
    }).format(date)
}

function NotificationRow({
    notification,
    onMarkAsRead,
}: {
    notification: NotificationResponse
    onMarkAsRead: (id: string) => void
}) {
    return (
        <button
            type="button"
            className={`notification-center__row${
                notification.isRead
                    ? ''
                    : ' notification-center__row--unread'
            }`}
            onClick={() => {
                if (!notification.isRead) {
                    onMarkAsRead(notification.id)
                }
            }}
        >
            <span className="notification-center__status" aria-hidden="true">
                {notification.isRead ? '' : '●'}
            </span>

            <span className="notification-center__row-content">
                <span className="notification-center__message">
                    {notification.message}
                </span>

                <span className="notification-center__time">
                    {formatNotificationTime(notification.createdAt)}
                </span>
            </span>
        </button>
    )
}

export default function NotificationCenter() {
    const [isOpen, setIsOpen] = useState(false)

    const {
        notifications,
        isLoading,
        isError,
        markAsRead,
        markAllAsRead,
        isMarkingAllAsRead,
    } = useNotifications()

    const recentNotifications = notifications.slice(0, 1)
    const unreadCount = notifications.filter(
        (notification) => !notification.isRead
    ).length

    return (
        <div className="notification-center">
            <button
                type="button"
                className="notification-center__preview"
                aria-expanded={isOpen}
                aria-controls="notification-center-history"
                onClick={() => setIsOpen((open) => !open)}
                title="Open notification history"
            >
                <span className="notification-center__preview-heading">
                    <span>Notifications</span>

                    {unreadCount > 0 && (
                        <span className="notification-center__count">
                            {unreadCount > 99 ? '99+' : unreadCount}
                        </span>
                    )}
                </span>

                <span className="notification-center__preview-items">
                    {recentNotifications.length > 0 ? (
                        recentNotifications.map((notification) => (
                            <span
                                key={notification.id}
                                className={`notification-center__preview-item${
                                    notification.isRead
                                        ? ''
                                        : ' notification-center__preview-item--unread'
                                }`}
                            >
                                <span className="notification-center__preview-dot">
                                    {notification.isRead ? '' : '●'}
                                </span>

                                <span className="notification-center__preview-message">
                                    {notification.message}
                                </span>
                            </span>
                        ))
                    ) : (
                        <span className="notification-center__empty-preview">
                            No notifications yet
                        </span>
                    )}
                </span>
            </button>

            {isOpen && (
                <section
                    id="notification-center-history"
                    className="notification-center__panel"
                    aria-label="Notification history"
                >
                    <div className="notification-center__panel-header">
                        <div>
                            <h2>Notification history</h2>
                            <p>
                                {unreadCount === 0
                                    ? 'You are all caught up.'
                                    : `${unreadCount} unread notification${
                                          unreadCount === 1 ? '' : 's'
                                      }`}
                            </p>
                        </div>

                        <button
                            type="button"
                            className="notification-center__mark-all"
                            disabled={
                                unreadCount === 0 || isMarkingAllAsRead
                            }
                            onClick={() => markAllAsRead()}
                        >
                            Mark all read
                        </button>
                    </div>

                    <div className="notification-center__list">
                        {isLoading ? (
                            <p className="notification-center__state">
                                Loading notifications…
                            </p>
                        ) : isError ? (
                            <p className="notification-center__state notification-center__state--error">
                                Could not load notifications. Close this panel
                                and try again
                            </p>
                        ) : notifications.length === 0 ? (
                            <p className="notification-center__state">
                                You don't have any notifications yet
                            </p>
                        ) : (
                            notifications.map((notification) => (
                                <NotificationRow
                                    key={notification.id}
                                    notification={notification}
                                    onMarkAsRead={markAsRead}
                                />
                            ))
                        )}
                    </div>

                    <div className="notification-center__panel-footer">
                        Showing the latest {notifications.length} notifications
                    </div>
                </section>
            )}
        </div>
    )
}