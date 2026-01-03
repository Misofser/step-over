import type { Goal } from "../../api/goals";
import './GoalList.css';

type Props = {
  goals: Goal[];
  onDelete?: (id: number) => void;
};

export default function GoalList({ goals, onDelete }: Props) {
  return (
    <div className="goal-list">
      <ul>
        {goals.map(goal => (
          <li key={goal.id}>
            <span
              className={`${goal.isCompleted ? "completed" : ""}`}
            >
              {goal.title}
            </span>
            <button
              className="delete-button"
              onClick={() => onDelete?.(goal.id)}
            >
              ❌
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}
