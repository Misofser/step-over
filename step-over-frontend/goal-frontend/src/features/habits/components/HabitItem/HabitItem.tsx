import type { Habit } from "../../types/habits.types";
import "./HabitItem.css";

type HabitItemProps = {
  habit: Habit;
  onToggle: (id: number, date: string) => Promise<void>;
  onOpenMenu: (id: number) => void;
};

export function HabitItem({habit, onToggle, onOpenMenu }: HabitItemProps) {
  const today = new Date().toISOString().slice(0, 10);

  return (
    <div className="habit">
      <div
        className="habit-main"
        onClick={() => onToggle(habit.id, today)}
      >
        <div
          className={`habit-toggle ${habit.isCompletedToday ? "done" : ""}`}
        >
          {habit.isCompletedToday ? "✔" : ""}
        </div>

        <div>
          <div className={`habit-title ${habit.isCompletedToday ? "completed" : ""}`}>
            {habit.title}
          </div>

          <div className="habit-meta">
            {habit.frequency}
          </div>
        </div>
      </div>

      <span className="habit-actions">
        <button
          type="button"
          className="habit-menu-btn"
          onClick={() => onOpenMenu(habit.id)}
        >
          ⋮
        </button>
      </span>
    </div>
  );
}
