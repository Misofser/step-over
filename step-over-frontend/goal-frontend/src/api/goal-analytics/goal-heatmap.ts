import { API_URL } from "../../config";
import type { GoalHeatmapDay } from "./goal-heatmap.types";

export async function fetchGoalHeatmap(goalId: number, days: number = 30
): Promise<GoalHeatmapDay[]> {
  const res = await fetch(
    `${API_URL}/goals/${goalId}/heatmap?days=${days}`,
    {
      credentials: "include",
    }
  );

  if (!res.ok) throw new Error("Failed to fetch goal heatmap");
  return res.json();
}
