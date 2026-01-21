import { API_URL } from '../config'
import type { User, UserToUpdate } from './users.types'

export async function fetchUsers(): Promise<User[]> {
  const res = await fetch(`${API_URL}/users`, {
    credentials: "include",
  });
  if (!res.ok) throw new Error("Failed to fetch users");
  return res.json();
}

export async function fetchUser(id: number): Promise<User> {
  const res = await fetch(`${API_URL}/users/${id}`, {
    credentials: "include"
  });
  if (!res.ok) throw new Error("Failed to fetch user");
  return res.json();
}

export async function addUser(data: {
  username: string;
  password: string;
}): Promise<User> {
  const res = await fetch(`${API_URL}/users`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
    credentials: "include",
  });

  if (!res.ok) {
    throw new Error("Failed to create user");
  }

  return res.json();
}

export async function deleteUser(id: number): Promise<void> {
  const res = await fetch(`${API_URL}/users/${id}`, {
    method: "DELETE",
    credentials: "include",
  });

  if (!res.ok) {
    throw new Error("Failed to delete user");
  }
}

export async function updateUser(id: number, dataToUpdate: UserToUpdate): Promise<void> {
  const res = await fetch(`${API_URL}/users/${id}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dataToUpdate),
    credentials: "include",
  });

  if (!res.ok) throw new Error("Failed to update user");
}
