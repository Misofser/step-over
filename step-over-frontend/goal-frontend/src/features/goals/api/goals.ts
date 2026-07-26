import { authenticatedFetch } from "../../../lib/api-client";
import type { Goal, GoalToCreate, DataToUpdate } from "../types/goals.types";

export async function fetchGoals(): Promise<Goal[]> {
  const res = await authenticatedFetch(`/goals`);
  if (!res.ok) throw new Error("Failed to fetch goals");
  return res.json();
}

export async function fetchGoal(id: number): Promise<Goal> {
  const res = await authenticatedFetch(`/goals/${id}`);
  if (!res.ok) throw new Error("Failed to fetch goal");
  return res.json();
}

export async function addGoal(goalToCreate: GoalToCreate): Promise<Goal> {
  const res = await authenticatedFetch(`/goals`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(goalToCreate),
  });

  if (!res.ok) throw new Error("Failed to add goal");

  return res.json();
}

export async function deleteGoal(id: number): Promise<void> {
  const res = await authenticatedFetch(`/goals/${id}`, {
    method: "DELETE",
  });

  if (!res.ok) throw new Error("Failed to delete goal");
}

export async function updateGoal(id: number, dataToUpdate: DataToUpdate): Promise<void> {
  const res = await authenticatedFetch(`/goals/${id}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dataToUpdate),
  });

  if (!res.ok) throw new Error("Failed to update goal");
}
