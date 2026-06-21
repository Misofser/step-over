export type TodayItemType = "Task" | "Habit";

export type TodayItem = {
  entityId: number;
  type: TodayItemType;
  title: string;
  isCompleted: boolean;
  goalId: number;
  goalTitle: string;
};

export type TodayDashboard = {
  pending: TodayItem[];
  completed: TodayItem[];
};
