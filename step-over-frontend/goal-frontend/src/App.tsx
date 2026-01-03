import { useEffect, useState } from "react";

import './styles/app.css'
import type { Goal } from './api/goals';
import GoalList from './components/GoalList';
import NewGoalForm from './components/NewGoalForm';
import { addGoal as apiAddGoal, fetchGoals } from "./api/goals";

function App() {
  const [goals, setGoals] = useState<Goal[]>([]);

  useEffect(() => {
    fetchGoals().then(setGoals);
  }, []);

  const addGoal = async (title: string) => {
    const newGoal = await apiAddGoal(title);
    setGoals(prev => [...prev, newGoal]);
  };

  return (
    <div className="app-container">
      <h1>StepOver</h1>
      <h2>Goals List</h2>
      <NewGoalForm onAddGoal={addGoal} />
      <GoalList goals={goals} />
    </div>
  );
}

export default App;
