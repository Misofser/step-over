import { API_URL } from '../config'
import type { User } from './users.types'

export async function fetchUsers(): Promise<User[]> {
  const res = await fetch(`${API_URL}/users`, {
    credentials: "include",
  });
  if (!res.ok) throw new Error("Failed to fetch users");
  return res.json();
}
