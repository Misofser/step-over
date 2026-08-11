import { authenticatedFetch, baseFetch } from '../../../api/api-client';
import type { ChangePasswordRequest } from '../types/auth.types';
import type { User } from '../../users';

export async function login(username: string, password: string): Promise<User> {
  const response = await baseFetch("/auth/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
  });

  if (!response.ok) throw new Error("Login failed");
  return response.json();
}

export async function getMe(): Promise<User> {
  const res = await authenticatedFetch("/auth/me");
  return res.json();
}

export function logout() {
  return baseFetch("/auth/logout", {
    method: "POST",
  });
}

export async function changePassword(data: ChangePasswordRequest): Promise<void> {
  const res = await authenticatedFetch("/auth/change-password", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  if (!res.ok) throw new Error("Failed to change password.");
}
