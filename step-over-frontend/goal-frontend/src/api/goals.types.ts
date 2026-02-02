export type Goal = {
  id: number;
  title: string;
  type: "Project" | "Process";
  isCompleted: boolean;
};

export type DataToUpdate = {
  title?: string;
  isCompleted?: boolean;
};

export type GoalToCreate = {
  title: string;
  type: "Project" | "Process";
};
