import './App.css'
import { BrowserRouter, Routes, Route, Navigate } from "react-router"
import { LoginPage } from './pages/LoginPage/LoginPage'
import { GoalsPage } from "./pages/GoalsPage"
import { ProtectedRoute } from "./auth/ProtectedRoute"
import { Navbar } from './components/Navbar/Navbar'

function App() {
  return (
    <BrowserRouter>
      <Navbar />
      <Routes>
        <Route path="/" element={<Navigate to="/login" />} />
        <Route path="/login" element={<LoginPage />} />
        <Route
          path="/goals"
          element={
            <ProtectedRoute>
              <GoalsPage />
            </ProtectedRoute>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
