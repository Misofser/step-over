import { useEffect, useState } from "react";

import {
  addThought as apiAddThought,
  fetchThoughts,
  updateThought as apiUpdateThought,
} from "../api/thoughts";
import type {
  Thought,
  ThoughtDataToUpdate,
  ThoughtToCreate,
} from "../types/thoughts.types";

export function useThoughts() {
  const [thoughts, setThoughts] = useState<Thought[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function load() {
      try {
        setLoading(true);

        const data = await fetchThoughts();

        setThoughts(data);
      } catch {
        setError("Could not load thoughts");
      } finally {
        setLoading(false);
      }
    }

    load();
  }, []);

  const addThought = async (data: ThoughtToCreate) => {
    const newThought = await apiAddThought(data);

    setThoughts((currentThoughts) => [...currentThoughts, newThought]);
  };

  const updateThought = async (id: number, data: ThoughtDataToUpdate) => {
    await apiUpdateThought(id, data);

    setThoughts((currentThoughts) =>
      currentThoughts.map((thought) =>
        thought.id === id ? { ...thought, ...data } : thought
      )
    );
  };

  return { thoughts, addThought, updateThought, loading, error };
}
