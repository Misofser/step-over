import { useEffect, useState } from "react";

import { API_URL } from "./config";

type Goal = { id: number; title: string };

export default function GoalList() {
  const [goals, setGoals] = useState<Goal[]>([]);

  useEffect(() => {
    fetch(`${API_URL}/Goals`)
      .then(res => res.json())
      .then(setGoals)
      .catch(console.error);
  }, []);

  return (
    <ul>
      {goals.map(goal => (
        <li key={goal.id}>{goal.title}</li>
      ))}
    </ul>
  );
}
