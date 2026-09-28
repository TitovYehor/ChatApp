import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'

import { login } from '../api/authApi'
import { useAuth } from '../features/auth/useAuth'
import { ApiError } from '../api/ApiError'

import './css/AuthPages.css'

function LoginPage() {
    const navigate = useNavigate()
    const { isAuthenticated, login: authenticate } = useAuth()

    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [error, setError] = useState<string | null>(null)
    const [isSubmitting, setIsSubmitting] = useState(false)

    if (isAuthenticated) {
        return <Navigate to="/chat" replace />
    }

    async function handleSubmit(
        event: FormEvent<HTMLFormElement>,
    ) {
        event.preventDefault()

        setError(null)
        setIsSubmitting(true)

        try {
            const response = await login({
                email,
                password,
            })

            authenticate(
                response.accessToken,
                response.user,
            )

            navigate('/chat', { replace: true })
        } catch (error) {
            if (
                error instanceof ApiError &&
                error.status === 401
            ) {
                setError('Invalid email or password')
            } else {
                setError('Unable to connect to the server')
            }
        } finally {
            setIsSubmitting(false)
        }
    }

    return (
        <main className="auth-page">
            <section className="auth-card">
                <div className="auth-card__brand">
                    <div className="auth-card__logo">
                        C
                    </div>

                    <span>Chat App</span>
                </div>

                <div className="auth-card__heading">
                    <h1>Welcome back</h1>
                    <p>
                        Log in to continue to your conversations.
                    </p>
                </div>

                <form
                    className="auth-form"
                    onSubmit={handleSubmit}
                >
                    <div className="auth-form__field">
                        <label htmlFor="email">
                            Email
                        </label>

                        <input
                            id="email"
                            type="email"
                            value={email}
                            onChange={(event) =>
                                setEmail(event.target.value)
                            }
                            placeholder="you@example.com"
                            required
                            autoComplete="email"
                        />
                    </div>

                    <div className="auth-form__field">
                        <label htmlFor="password">
                            Password
                        </label>

                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={(event) =>
                                setPassword(event.target.value)
                            }
                            placeholder="Enter your password"
                            required
                            autoComplete="current-password"
                        />
                    </div>

                    {error && (
                        <p
                            className="auth-form__error"
                            role="alert"
                        >
                            {error}
                        </p>
                    )}

                    <button
                        className="auth-form__submit"
                        type="submit"
                        disabled={isSubmitting}
                    >
                        {isSubmitting
                            ? 'Logging in...'
                            : 'Login'}
                    </button>
                </form>

                <p className="auth-card__footer">
                    Don't have an account?{' '}
                    <Link to="/register">
                        Register
                    </Link>
                </p>
            </section>
        </main>
    )
}

export default LoginPage