import { useEffect, useState } from "react";
import { fetchTasks, addTask as apiAddTask } from "../api/goal-tasks";

import type { Task, TaskToCreate } from "../api/goal-tasks.types";

export function useTasks(goalId: number) {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function load() {
      try {
        setLoading(true);

        const data = await fetchTasks(goalId);

        setTasks(data);
      } catch {
        setError("Failed to load habits");
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [goalId]);

  const addTask = async (data: TaskToCreate) => {
    const newTask = await apiAddTask(goalId, data);
    setTasks(prev => [...prev, newTask]);
  };

  return { tasks, setTasks, addTask, loading, error };
}
