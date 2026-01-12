import { API_URL } from '../config'
import type { Goal, DataToUpdate } from './goals.types'

export async function fetchGoals(): Promise<Goal[]> {
  const res = await fetch(`${API_URL}/goals`, {
    credentials: "include",
  });
  if (!res.ok) throw new Error("Failed to fetch goals");
  return res.json();
}

export async function fetchGoal(id: number): Promise<Goal> {
  const res = await fetch(`${API_URL}/goals/${id}`, {
    credentials: "include"
  });
  if (!res.ok) throw new Error("Failed to fetch goal");
  return res.json();
}

export async function addGoal(title: string): Promise<Goal> {
  const res = await fetch(`${API_URL}/goals`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ title }),
    credentials: "include",
  });

  if (!res.ok) {
    throw new Error("Failed to add goal");
  }

  return res.json();
}

export async function deleteGoal(id: number): Promise<void> {
  const res = await fetch(`${API_URL}/goals/${id}`, {
    method: "DELETE",
    credentials: "include",
  });

  if (!res.ok) {
    throw new Error("Failed to delete goal");
  }
}

export async function updateGoal(id: number, dataToUpdate: DataToUpdate): Promise<void> {
  const res = await fetch(`${API_URL}/goals/${id}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dataToUpdate),
    credentials: "include",
  });

  if (!res.ok) throw new Error("Failed to update goal");
}
