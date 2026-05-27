export type Habit = {
  id: number;
  title: string;
  frequency: HabitFrequency;
  isCompletedToday: boolean;
};

export type HabitToCreate = {
  title: string;
  frequency: HabitFrequency;
};

export type HabitFrequency = "Daily" | "Weekly" | "Monthly";
