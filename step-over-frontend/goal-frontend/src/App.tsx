import { useEffect, useState } from "react";

import './App.css'
import type { Goal } from './api/goals';
import GoalList from './components/GoalList/GoalList';
import NewGoalForm from './components/NewGoalForm/NewGoalForm';
import { addGoal as apiAddGoal, fetchGoals, deleteGoal } from "./api/goals";

function App() {
  const [goals, setGoals] = useState<Goal[]>([]);

  useEffect(() => {
    fetchGoals().then(setGoals);
  }, []);

  const addGoal = async (title: string) => {
    const newGoal = await apiAddGoal(title);
    setGoals(prev => [...prev, newGoal]);
  };

  const handleDeleteGoal = async (id: number) => {
    try {
      await deleteGoal(id);
      setGoals(prev => prev.filter(goal => goal.id !== id));
    } catch (e) {
      alert("Could not delete goal");
    }
  };

  return (
    <div className="app-container">
      <h1>StepOver</h1>
      <h2>Goals List</h2>
      <NewGoalForm onAddGoal={addGoal} />
      <GoalList
        goals={goals}
        onDelete={handleDeleteGoal}
      />
    </div>
  );
}

export default App;
