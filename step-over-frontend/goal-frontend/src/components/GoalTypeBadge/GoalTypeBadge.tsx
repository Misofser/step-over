import type { GoalType } from "../../api/goals.types";
import "./GoalTypeBadge.css";

export function GoalTypeBadge({ type }: { type: GoalType }) {
  return (
    <span className={`goal-bage ${type.toLowerCase()}`}>
      {type}
    </span>
  );
}
