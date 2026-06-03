import { API_URL } from '../config'
import type { Habit, HabitToCreate } from './habits.types'

export async function fetchHabits(goalId: number): Promise<Habit[]> {
  const res = await fetch(`${API_URL}/goals/${goalId}/habits`, {
    credentials: "include",
  });
  if (!res.ok) throw new Error("Failed to load habits");
  return res.json();
}

export async function addHabit(goalId: number, habitToCreate: HabitToCreate): Promise<Habit> {
  const res = await fetch(`${API_URL}/goals/${goalId}/habits`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(habitToCreate),
    credentials: "include",
  });

  if (!res.ok) throw new Error("Failed to add habit");

  return res.json();
}

export async function toggleHabitCompletion(id: number, date: string): Promise<void> {
  const res = await fetch(`${API_URL}/habits/${id}/toggle`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ date: date }),
    credentials: "include",
  });

  if (!res.ok) throw new Error("Failed to toggle habit completion");
}

export async function deleteHabit(id: number): Promise<void> {
  const res = await fetch(`${API_URL}/habits/${id}`, {
    method: "DELETE",
    credentials: "include",
  });

  if (!res.ok) {
    throw new Error("Failed to delete Habit");
  }
}
