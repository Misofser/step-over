export type GoalHeatmapDay = {
  date: string;
  completedHabits: number;
  totalHabits: number;
}

export type HeatmapCell = GoalHeatmapDay & {
  weekday: number;
  weekIndex: number;
};
