import { Link } from "react-router";
import type { TodayItem } from "../../types/today.types";
import "./TodayItemRow.css";

type Props = {
  item: TodayItem;
  onToggle: (item: TodayItem) => void;
};

export function TodayItemRow({ item, onToggle }: Props) {
  return (
    <div className="today-item">
      <div className="today-item-main">
        <div
          className={`today-item-indicator ${item.type.toLowerCase()} ${
            item.isCompleted ? "done" : ""
          }`}
          onClick={() => onToggle(item)}
        >
          {item.isCompleted ? "✔" : ""}
        </div>

        <div className="today-item-content">
          <div className={item.isCompleted ? "completed" : ""}>
            {item.title}
          </div>

          <div className="today-item-meta">
            <span className="today-meta-label">Goal:</span>
            <Link className="today-item-goal" to={`/goals/${item.goalId}`}>
              <span>{item.goalTitle}</span>
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}
