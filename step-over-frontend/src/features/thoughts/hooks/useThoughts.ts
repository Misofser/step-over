import { useEffect, useState } from "react";

import { addThought as apiAddThought, fetchThoughts } from "../api/thoughts";
import type { Thought, ThoughtToCreate } from "../types/thoughts.types";

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

  return { thoughts, addThought, loading, error };
}
