import { useEffect, useState, useContext } from "react"
import type { Goal, GoalToCreate } from '../api/goals.types'
import GoalList from '../components/GoalList/GoalList'
import NewGoalForm from '../components/NewGoalForm/NewGoalForm'
import { AuthContext } from "../auth/AuthContext"
import { addGoal as apiAddGoal, fetchGoals } from "../api/goals"

export function GoalsPage() {
  const [goals, setGoals] = useState<Goal[]>([]);
  const { user } = useContext(AuthContext);

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
      {user?.role === "Admin" && (
        <NewGoalForm onAddGoal={addGoal} />
      )}
      <GoalList goals={goals} />
    </div>
  );
}
