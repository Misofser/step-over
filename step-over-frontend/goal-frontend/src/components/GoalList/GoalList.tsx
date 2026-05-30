import { Link } from "react-router";
import type { Goal } from "../../api/goals.types";
import './GoalList.css';

type Props = {
  goals: Goal[];
};

export default function GoalList({ goals }: Props) {

  return (
    <div className="goal-list">
      {goals.map(goal => (
        <Link to={`/goals/${goal.id}`} key={goal.id} className="goal-card">
          <div className="goal-header">
            <span className={`goal-title ${goal.isCompleted ? "completed" : ""}`}>{goal.title}</span>
            <span className="goal-type">[{goal.type}]</span>
          </div>
          {goal.isCompleted && <div className="goal-completed-badge">✔</div>}
        </Link>
      ))}
    </div>
  );
}
