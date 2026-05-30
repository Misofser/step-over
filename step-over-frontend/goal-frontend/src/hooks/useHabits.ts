import { useEffect, useState } from "react";
import { fetchHabits, addHabit as apiAddHabit } from "../api/habits";

import type { Habit, HabitToCreate } from "../api/habits.types";

export function useHabits(goalId: number) {
  const [habits, setHabits] = useState<Habit[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function load() {
      try {
        setLoading(true);

        const data = await fetchHabits(goalId);

        setHabits(data);
      } catch {
        setError("Failed to load habits");
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [goalId]);

  const addHabit = async (data: HabitToCreate) => {
    const newHabit = await apiAddHabit(goalId, data);
    setHabits(prev => [...prev, newHabit]);
  };

  return { habits, setHabits, addHabit, loading, error };
}

