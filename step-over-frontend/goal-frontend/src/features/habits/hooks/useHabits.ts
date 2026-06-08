import { useEffect, useState } from "react";
import { fetchHabits, addHabit as apiAddHabit, toggleHabitCompletion, deleteHabit } from "../api/habits";

import type { Habit, HabitToCreate } from "../types/habits.types";

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

  const toggleHabit = async (id: number, date: string) => {
    const today = new Date().toISOString().slice(0, 10);

    try {
      await toggleHabitCompletion(id, date);
      if (date === today) {
        setHabits(prev =>
          prev.map(h =>
            h.id === id
              ? { ...h, isCompletedToday: !h.isCompletedToday }
              : h
          )
        );
      }
    } catch {
      alert("Error toggling habit");
    }
  };

  const removeHabit = async (id: number) => {
    try {
      await deleteHabit(id);
      setHabits((prev) => prev.filter((h) => h.id !== id));
    } catch {
      alert("Could not delete habit");
    }
  };

  return { habits, setHabits, addHabit, toggleHabit, removeHabit, loading, error };
}
