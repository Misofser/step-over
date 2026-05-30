import './App.css'
import { BrowserRouter, Routes, Route, Navigate } from "react-router"
import { LoginPage } from './pages/LoginPage/LoginPage'
import { GoalsPage } from "./pages/GoalsPage"
import { GoalPage } from './pages/GoalPage/GoalPage'
import { AdminUsersPage } from './pages/AdminUsersPage/AdminUsersPage'
import { ProtectedRoute } from "./auth/ProtectedRoute"
import { Navbar } from './components/Navbar/Navbar'
import { NotFoundPage } from './pages/NotFoundPage/NotFoundPage'

function App() {
  return (
    <BrowserRouter>
      <Navbar />
      <Routes>
        <Route path="/" element={<Navigate to="/login" />} />
        <Route path="/login" element={<LoginPage />} />
        <Route
          path="/admin/users"
          element={
            <ProtectedRoute role="Admin">
              <AdminUsersPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/goals"
          element={
            <ProtectedRoute>
              <GoalsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/goals/:goalId"
          element={
            <ProtectedRoute>
              <GoalPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="*"
          element={
            <ProtectedRoute>
              <NotFoundPage />
            </ProtectedRoute>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
