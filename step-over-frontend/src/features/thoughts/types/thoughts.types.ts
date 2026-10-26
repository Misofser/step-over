export type Thought = {
  id: number;
  content: string;
  status: ThoughtStatus;
};

export type ThoughtToCreate = {
  content: string;
};

export type ThoughtDataToUpdate = {
  content?: string;
  status?: ThoughtStatus;
};

export type ThoughtStatus = "Inbox" | "Archived";
