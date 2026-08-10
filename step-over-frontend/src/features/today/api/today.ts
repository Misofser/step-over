import { API_URL } from "../../../config";
import type { TodayDashboard } from "../types/today.types";

export async function getToday(): Promise<TodayDashboard> {
  const res = await fetch(`${API_URL}/today`, {
    credentials: "include",
  });
  if (!res.ok) throw new Error("Failed to load today's data");
  return res.json();
}
