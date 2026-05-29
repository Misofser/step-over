import type { Habit } from "../../api/habits.types";
import "./HabitItem.css";

type HabitItemProps = {
  habit: Habit;
  onToggle: (habitId: number) => void;
};

export function HabitItem({ habit, onToggle }: HabitItemProps) {
  return (
    <div className="habit">
      <div
        className="habit-main"
        onClick={() => onToggle(habit.id)}
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
          <div className="habit-meta">{habit.frequency}</div>
        </div>
      </div>
    </div>
  );
};
