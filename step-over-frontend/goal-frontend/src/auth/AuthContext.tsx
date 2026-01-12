import { createContext, useState, useEffect } from "react"
import type { ReactNode } from "react"
import { getMe, logout as logoutApi } from "../api/auth"

interface AuthContextType {
  isAuthenticated: boolean;
  userRole: string | null;
  login: (role: string) => void;
  logout: () => void;
  loading: boolean,
}

export const AuthContext = createContext<AuthContextType>(
  {} as AuthContextType
);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [userRole, setUserRole] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function checkAuth() {
      try {
        const data = await getMe();
        setUserRole(data.role);
      } catch {
        setUserRole(null);
      } finally {
        setLoading(false);
      }
    }

    checkAuth();
  }, []);

  const login = (role: string) => setUserRole(role);

  const logout = async () => {
    try {
      await logoutApi();
    } finally {
      setUserRole(null);
    }
  };

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated: !!userRole,
        userRole,
        login,
        logout,
        loading,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}
