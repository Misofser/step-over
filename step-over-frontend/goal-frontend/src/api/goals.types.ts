export type Goal = {
  id: number;
  title: string;
  type: GoalType;
  isCompleted: boolean;
};

export type DataToUpdate = {
  title?: string;
  isCompleted?: boolean;
};

export type GoalToCreate = {
  title: string;
  type: GoalType;
};

export type GoalType = "Project" | "Process";
