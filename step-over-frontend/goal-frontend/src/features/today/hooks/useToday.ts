import { useEffect, useState } from "react";
import { getToday } from "../api/today";
import type { TodayDashboard, TodayItem } from "../types/today.types";
import { updateTaskCompletion } from "../../../api/goal-tasks";
import { toggleHabitCompletion } from "../../habits";

export function useToday() {
  const [data, setData] = useState<TodayDashboard | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const today = new Date().toISOString().slice(0, 10);

  useEffect(() => {
    async function load() {
      try {
        setLoading(true);
        const data = await getToday();
        setData(data);
      } catch {
        setError("Failed to load today's data");
      } finally {
        setLoading(false);
      }
    }

    load();
  }, []);

  const toggleItem = async (item: TodayItem) => {
    const originalData = data;

    setData(prev => {
      if (!prev) return prev;

      const updated = {
        ...item,
        isCompleted: !item.isCompleted,
      };

      const removeFromLists = (list: TodayItem[]) =>
        list.filter(i => i.entityId !== item.entityId);

      const pending = removeFromLists(prev.pending);
      const completed = removeFromLists(prev.completed);

      if (updated.isCompleted) {
        completed.unshift(updated);
      } else {
        pending.unshift(updated);
      }

      return { pending, completed };
    });

    try {
      if (item.type === "Task") {
        await updateTaskCompletion(item.entityId, !item.isCompleted);
      }

      if (item.type === "Habit") {
        await toggleHabitCompletion(item.entityId, today);
      }
    } catch (e) {
      setData(originalData);
    }
  };

  return { data, loading, error, toggleItem };
}
