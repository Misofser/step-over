import { API_URL } from '../config';

export type Goal = {
  id: number;
  title: string;
  isCompleted: boolean;
  createdAt: string;
};

export async function fetchGoals(): Promise<Goal[]> {
  const res = await fetch(`${API_URL}/goals`);
  if (!res.ok) throw new Error("Failed to fetch goals");
  return res.json();
}

export async function addGoal(title: string): Promise<Goal> {
  const res = await fetch(`${API_URL}/goals`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ title }),
  });

  if (!res.ok) {
    throw new Error("Failed to add goal");
  }

  return res.json();
}
