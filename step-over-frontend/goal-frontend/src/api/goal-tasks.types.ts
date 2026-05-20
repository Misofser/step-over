export type Task = {
  id: number;
  title: string;
  isCompleted: boolean;
}

export type TaskToUpdate = {
  title: string;
}

export type TaskToCreate = {
  title: string;
}
