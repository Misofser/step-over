import type { Goal } from '../api/goals';

type Props = {
  goals: Goal[];
};

export default function GoalList({ goals }: Props) {
  return (
    <div>
      <ul>
        {goals.map(goal => (
          <li key={goal.id}>{goal.title}</li>
        ))}
      </ul>
    </div>
  );
}
