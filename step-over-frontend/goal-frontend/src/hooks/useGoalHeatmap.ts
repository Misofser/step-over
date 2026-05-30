import { useEffect, useState } from "react";
import { fetchGoalHeatmap } from "../api/goal-analytics/goal-heatmap";
import type { HeatmapCell } from "../api/goal-analytics/goal-heatmap.types";
import { transformHeatmap } from "../utils/heatmap";

export function useGoalHeatmap(goalId: number, days: number = 30) {
  const [data, setData] = useState<HeatmapCell[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!goalId || Number.isNaN(goalId)) return;

    let isActive = true;

    async function load() {
      try {
        setLoading(true);
        setError(null);

        const result = await fetchGoalHeatmap(goalId, days);
        const transformed = transformHeatmap(result);

        if (!isActive) return;

        setData(transformed);
      } catch {
        if (!isActive) return;
        setError("Failed to load heatmap");
      } finally {
        if (isActive) setLoading(false);
      }
    }

    load();

    return () => {
      isActive = false;
    };
  }, [goalId, days]);

  return {
    data,
    loading,
    error,
  };
}
