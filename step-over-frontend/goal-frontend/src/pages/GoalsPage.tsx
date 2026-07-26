import { useEffect, useState } from "react"
import type { Goal, GoalToCreate } from '../api/goals.types'
import GoalList from '../components/GoalList/GoalList'
import NewGoalForm from '../components/NewGoalForm/NewGoalForm'
import { addGoal as apiAddGoal, fetchGoals } from "../api/goals"

export function GoalsPage() {
  const [goals, setGoals] = useState<Goal[]>([]);

  useEffect(() => {
    fetchGoals().then(setGoals);
  }, []);

  const addGoal = async (goalToCreate: GoalToCreate) => {
    const newGoal = await apiAddGoal(goalToCreate);
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
