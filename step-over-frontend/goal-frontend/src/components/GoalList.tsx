import type { Goal } from '../api/goals';

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
            {goal.title}
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
