import { authenticatedFetch, baseFetch } from '../../../lib/api-client';
import type { User } from '../../../api/users.types'

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
