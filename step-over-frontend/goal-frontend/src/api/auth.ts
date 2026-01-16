import { API_URL } from '../config'
import type { User } from './users.types'

export async function login(username: string, password: string): Promise<User> {
  const response = await fetch(`${API_URL}/auth/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    credentials: "include",
    body: JSON.stringify({ username, password })
  });

  if (!response.ok) throw new Error("Login failed");

  return response.json();
}

export async function getMe(): Promise<User> {
  const res = await fetch(`${API_URL}/auth/me`, {
    credentials: "include"
  });

  if (!res.ok) throw new Error("Not authenticated");

  return res.json();
}

export async function logout() {
   return fetch(`${API_URL}/auth/logout`, {
    method: "POST",
    credentials: "include"
  });
};
