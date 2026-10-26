import { authenticatedFetch } from "@/api/api-client";
import type { Thought, ThoughtDataToUpdate, ThoughtToCreate } from "../types/thoughts.types";

export async function fetchThoughts(): Promise<Thought[]> {
  const res = await authenticatedFetch(`/thoughts`);
  if (!res.ok) throw new Error("Failed to fetch thoughts");
  return res.json();
}

export async function fetchThought(id: number): Promise<Thought> {
  const res = await authenticatedFetch(`/thoughts/${id}`);
  if (!res.ok) throw new Error("Failed to fetch thought");
  return res.json();
}

export async function addThought(thoughtToCreate: ThoughtToCreate): Promise<Thought> {
  const res = await authenticatedFetch(`/thoughts`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(thoughtToCreate),
  });

  if (!res.ok) throw new Error("Failed to add thought");

  return res.json();
}

export async function updateThought(id: number, dataToUpdate: ThoughtDataToUpdate): Promise<void> {
  const res = await authenticatedFetch(`/thoughts/${id}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dataToUpdate),
  });

  if (!res.ok) throw new Error("Failed to update thought");
}
