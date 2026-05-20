import { API_URL } from '../config'
import type { Task, TaskToUpdate, TaskToCreate } from './goal-tasks.types'

export async function fetchTasks(goalId: number): Promise<Task[]> {
  const res = await fetch(`${API_URL}/goals/${goalId}/tasks`, {
    credentials: "include",
  });
  if (!res.ok) throw new Error("Failed to load tasks");
  return res.json();
}

export async function fetchTask(id: number): Promise<Task> {
  const res = await fetch(`${API_URL}/tasks/${id}`, {
    credentials: "include"
  });
  if (!res.ok) throw new Error("Failed to fetch task");
  return res.json();
}

export async function addTask(goalId: number, taskToCreate: TaskToCreate): Promise<Task> {
  const res = await fetch(`${API_URL}/goals/${goalId}/tasks`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(taskToCreate),
    credentials: "include",
  });

  if (!res.ok) throw new Error("Failed to add task");

  return res.json();
}

export async function updateTaskCompletion(id: number, isCompleted: boolean): Promise<void> {
  const res = await fetch(`${API_URL}/tasks/${id}/completion`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ isCompleted: isCompleted }),
    credentials: "include",
  });

  if (!res.ok) throw new Error("Failed to update completion");
}

export async function updateTask(id: number, dataToUpdate: TaskToUpdate): Promise<void> {
  const res = await fetch(`${API_URL}/tasks/${id}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dataToUpdate),
    credentials: "include",
  });

  if (!res.ok) throw new Error("Failed to update task");
}

export async function deleteTask(id: number): Promise<void> {
  const res = await fetch(`${API_URL}/tasks/${id}`, {
    method: "DELETE",
    credentials: "include",
  });

  if (!res.ok) {
    throw new Error("Failed to delete task");
  }
}
