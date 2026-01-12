import { Navigate } from "react-router"
import { useContext } from "react"
import type { ReactNode } from "react"
import { AuthContext } from "./AuthContext"


interface ProtectedRouteProps {
  children: ReactNode
  role?: string
}

export function ProtectedRoute({ children, role }: ProtectedRouteProps) {
  const { isAuthenticated, userRole, loading } = useContext(AuthContext);

  if (loading) {
    return <div>Loading...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (role && userRole !== role) {
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
}
