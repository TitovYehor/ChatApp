import {
    BrowserRouter,
    Navigate,
    Route,
    Routes,
} from 'react-router-dom'

import ProtectedRoute from './components/ProtectedRoute'
import PublicRoute from './components/PublicRoute'

import ChatPage from './pages/ChatPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import ProfilePage from './pages/ProfilePage'
import UserProfilePage from './pages/UserProfilePage'
import FriendsPage from './pages/FriendsPage'

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<PublicRoute />}>
                    <Route
                        path="/login"
                        element={<LoginPage />}
                    />

                    <Route
                        path="/register"
                        element={<RegisterPage />}
                    />
                </Route>

                <Route element={<ProtectedRoute />}>
                    <Route
                        path="/chat"
                        element={<ChatPage />}
                    />

                    <Route
                        path="/friends"
                        element={<FriendsPage />}
                    />

                    <Route
                        path="/profile"
                        element={<ProfilePage />}
                    />

                    <Route
                        path="/users/:userId"
                        element={<UserProfilePage />}
                    />
                </Route>

                <Route
                    path="*"
                    element={
                        <Navigate
                            to="/login"
                            replace
                        />
                    }
                />
            </Routes>
        </BrowserRouter>
    )
}

export default App