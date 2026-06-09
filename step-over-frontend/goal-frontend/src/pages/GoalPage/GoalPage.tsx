import { useState } from "react";
import { useParams, useNavigate } from "react-router";

import { useGoal } from "../../hooks/useGoal";
import { useTasks } from "../../hooks/useTasks";
import { useGoalHeatmap } from "../../hooks/useGoalHeatmap";
import { deleteGoal, updateGoal } from "../../api/goals";
import { GoalHeader } from "../../components/GoalHeader/GoalHeader";
import { GoalHeatmap } from "../../components/GoalHeatmap/GoalHeatmap";
import { TaskList } from "../../components/TaskList/TaskList";
import { HabitList, useHabits } from "../../features/habits";
import { Modal } from "../../components/Modal/Modal";
import { EditGoalForm } from "../../components/EditGoalForm/EditGoalForm";
import { GoalProgress } from "../../components/GoalProgress/GoalProgress";
import { GoalStatus } from "../../components/GoalStatus/GoalStatus";
import "./GoalPage.css";

export function GoalPage() {
  const { goalId } = useParams<{ goalId: string }>();
  const [editingGoalId, setEditingGoalId] = useState<number | null>(null);

  const id = Number(goalId);

  const { goal, setGoal, loading: goalLoading, error: goalError } = useGoal(id);
  const { tasks, setTasks, addTask, loading: tasksLoading, error: tasksError } = useTasks(id);
  const { habits, addHabit, toggleHabit, removeHabit, loading: habitsLoading, error: habitsError } = useHabits(id);
  const { data: heatmap, loading: heatmapLoading, error: heatmapError } = useGoalHeatmap(id, 90);

  const loading = goalLoading || tasksLoading || habitsLoading || heatmapLoading;
  const error = goalError || tasksError || habitsError || heatmapError;

  const totalTasks = tasks.length;
  const completedTasks = tasks.filter(t => t.isCompleted).length;

  const navigate = useNavigate();

  const handleDeleteGoal = async () => {
    if (!goal) return;

    try {
      await deleteGoal(goal.id);
      navigate("/goals");
    } catch (e) {
      alert("Could not delete goal");
    }
  };
  
  const handleToggleCompletion = async () => {
    if (!goal) return;

    try {
      const updated = { ...goal, isCompleted: !goal.isCompleted, };
      await updateGoal(goal.id, { isCompleted: updated.isCompleted });

      setGoal(updated);
    } catch (e) {
      alert("Could not update goal");
    }
  };
  
  const handleSavedGoal = (id: number, newTitle: string) => {
    if (!goal) return;

    setGoal(prev =>
      prev ? { ...prev, title: newTitle } : prev
    );
  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p>{error}</p>;
  if (!goal) return null;

  return (
    <div className={`app-container ${goal.isCompleted ? "goal-completed" : ""}`}>
      <GoalHeader
        goal={goal}
        onRename={goal => setEditingGoalId(goal.id)}
        onDelete={handleDeleteGoal}
      />
      <GoalStatus
        isCompleted={goal?.isCompleted ?? false}
        onToggleCompletion={handleToggleCompletion}
      />
      <GoalProgress
        completedTasks={completedTasks}
        totalTasks={totalTasks}
      />
      <GoalHeatmap days={heatmap} />
      <TaskList
        tasks={tasks}
        setTasks={setTasks}
        addTask={addTask}
      />
      <HabitList
        habits={habits}
        addHabit={addHabit}
        onToggle={toggleHabit}
        onDelete={removeHabit}
      />
      {editingGoalId && (
        <Modal title="Edit Goal">
          <EditGoalForm
            goalId={editingGoalId}
            onClose={() => setEditingGoalId(null)}
            onSave={handleSavedGoal}
          />
        </Modal>
      )}
    </div>
  );
}
