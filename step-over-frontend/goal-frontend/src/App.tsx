import { useEffect, useState } from "react";

import './App.css'
import type { Goal } from './api/goals.types';
import GoalList from './components/GoalList/GoalList';
import { EditGoalModal } from "./components/EditGoalModal/EditGoalModal";
import NewGoalForm from './components/NewGoalForm/NewGoalForm';
import { addGoal as apiAddGoal, fetchGoals, deleteGoal, updateGoal } from "./api/goals";

function App() {
  const [goals, setGoals] = useState<Goal[]>([]);
  const [editingGoalId, setEditingGoalId] = useState<number | null>(null);

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

  const handleToggleGoal = async (goal: Goal) => {
    try {
      await updateGoal(goal.id, {isCompleted: !goal.isCompleted});
      setGoals(prev =>
        prev.map(g =>
          g.id === goal.id ? { ...g, isCompleted: !goal.isCompleted } : g
        )
      );
    } catch (e) {
      alert("Could not update goal")
    }
  };

  const handleSavedGoal = (id: number, newTitle: string) => {
    setGoals(prev =>
      prev.map(g =>
        g.id === id ? { ...g, title: newTitle } : g
      )
    );
  };

  return (
    <div className="app-container">
      <h1>StepOver</h1>
      <h2>Goals List</h2>
      <NewGoalForm onAddGoal={addGoal} />
      <GoalList
        goals={goals}
        onDelete={handleDeleteGoal}
        onToggle={handleToggleGoal}
        onEdit={goal => setEditingGoalId(goal.id)}
      />    
      {editingGoalId && (
        <EditGoalModal
          goalId={editingGoalId}
          onClose={() => setEditingGoalId(null)}
          onSave={handleSavedGoal}
        />
      )}
    </div>
  );
}

export default App;
