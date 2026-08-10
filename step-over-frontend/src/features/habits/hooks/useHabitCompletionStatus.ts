import { useCallback, useState } from "react";
import { getHabitCompletionStatus } from "../api/habits";

export function useHabitCompletionStatus(habitId: number) {
  const [isCompleted, setIsCompleted] = useState(false);
  const [loading, setLoading] = useState(false);

  const loadStatus = useCallback(async (date: string) => {
    try {
      setLoading(true);

      const status = await getHabitCompletionStatus(habitId, date);
      setIsCompleted(status.isCompleted);
    } finally {
      setLoading(false);
    }
  }, [habitId]);

  return {
    isCompleted,
    setIsCompleted,
    loading,
    loadStatus,
  };
}
