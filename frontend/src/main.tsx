import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { QueryClientProvider } from '@tanstack/react-query'

import './index.css'
import App from './App.tsx'
import { AuthProvider } from './features/auth/AuthProvider'
import { queryClient } from './lib/queryClient'
import AuthenticatedApp from './components/AuthenticatedApp'

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <AuthProvider>
            <QueryClientProvider client={queryClient}>
                <AuthenticatedApp>
                    <App />
                </AuthenticatedApp>
            </QueryClientProvider>
        </AuthProvider>
    </StrictMode>,
)