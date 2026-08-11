import { authenticatedFetch } from "@/api/api-client";
import type { TodayDashboard } from "../types/today.types";

export async function getToday(): Promise<TodayDashboard> {
  const res = await authenticatedFetch("/today");
  if (!res.ok) throw new Error("Failed to load today's data");
  return res.json();
}
