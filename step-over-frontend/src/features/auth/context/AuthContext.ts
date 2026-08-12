import { createContext } from "react";
import type { User } from "@/features/users";

export type AuthContextType = {
  isAuthenticated: boolean;
  user: User | null;
  login: (user: User) => void;
  logout: () => void;
  loading: boolean,
}

export const AuthContext = createContext<AuthContextType>(
  {} as AuthContextType
);
