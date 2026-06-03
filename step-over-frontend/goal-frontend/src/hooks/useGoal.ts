import { useEffect, useState } from "react";
import { fetchGoal } from "../api/goals";

import type { Goal } from "../api/goals.types";

export function useGoal(goalId: number) {
  const [goal, setGoal] = useState<Goal | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function load() {
      try {
        setLoading(true);

        const data = await fetchGoal(goalId);

        setGoal(data);
      } catch {
        setError("Failed to load goal");
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [goalId]);

  return {
    goal,
    loading,
    error,
    setGoal,
  };
}
