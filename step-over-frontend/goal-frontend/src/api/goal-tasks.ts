import { authenticatedFetch } from '../lib/api-client';
import type { Task, TaskToUpdate, TaskToCreate } from './goal-tasks.types'

export async function fetchTasks(goalId: number): Promise<Task[]> {
  const res = await authenticatedFetch(`/goals/${goalId}/tasks`);
  if (!res.ok) throw new Error("Failed to load tasks");
  return res.json();
}

export async function fetchTask(id: number): Promise<Task> {
  const res = await authenticatedFetch(`/tasks/${id}`);
  if (!res.ok) throw new Error("Failed to fetch task");
  return res.json();
}

export async function addTask(goalId: number, taskToCreate: TaskToCreate): Promise<Task> {
  const res = await authenticatedFetch(`/goals/${goalId}/tasks`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(taskToCreate),
  });

  if (!res.ok) throw new Error("Failed to add task");

  return res.json();
}

export async function updateTaskCompletion(id: number, isCompleted: boolean): Promise<void> {
  const res = await authenticatedFetch(`/tasks/${id}/completion`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ isCompleted: isCompleted }),
  });

  if (!res.ok) throw new Error("Failed to update completion");
}

export async function updateTask(id: number, dataToUpdate: TaskToUpdate): Promise<void> {
  const res = await authenticatedFetch(`/tasks/${id}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dataToUpdate),
  });

  if (!res.ok) throw new Error("Failed to update task");
}

export async function deleteTask(id: number): Promise<void> {
  const res = await authenticatedFetch(`/tasks/${id}`, {
    method: "DELETE",
  });

  if (!res.ok) throw new Error("Failed to delete task");
}
