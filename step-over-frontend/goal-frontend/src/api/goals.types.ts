export type Goal = {
  id: number;
  title: string;
  isCompleted: boolean;
};

export type DataToUpdate = {
  title?: string;
  isCompleted?: boolean;
};
