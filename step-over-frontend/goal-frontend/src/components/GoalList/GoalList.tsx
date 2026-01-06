import type { Goal } from "../../api/goals.types";
import './GoalList.css';

type Props = {
  goals: Goal[];
  onDelete?: (id: number) => void;
  onToggle?: (goal: Goal) => void;
  onEdit: (goal: Goal) => void;
};

export default function GoalList({ goals, onDelete, onToggle, onEdit }: Props) {
  return (
    <div className="goal-list">
      <ul>
        {goals.map(goal => (
          <li key={goal.id}>
            <span className="goal-checkbox">
              <input
                id={`goal-${goal.id}`}
                type="checkbox"
                checked={goal.isCompleted}
                onChange={() => onToggle?.(goal)}
              />
              <label
                htmlFor={`goal-${goal.id}`}
                className={`${goal.isCompleted ? "completed" : ""}`}
              >
                {goal.title}
              </label>
            </span>
            <span className="buttons-block">
              <button
                className="edit-button"
                onClick={() => onEdit(goal)}
              >
                ✏️
              </button>
              <button
                className="delete-button"
                onClick={() => onDelete?.(goal.id)}
              >
                ❌
              </button>
            </span>
          </li>
        ))}
      </ul>
    </div>
  );
}
