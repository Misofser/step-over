import { useContext } from "react";

import type { Habit } from "../../api/habits.types";
import { Button } from "../Button/Button";
import { AuthContext } from "../../auth/AuthContext";
import "./HabitItem.css";

type HabitItemProps = {
  habit: Habit;
  onToggle: (id: number) => void;
  onDelete: (id: number) => void;
};

export function HabitItem({ habit, onToggle, onDelete }: HabitItemProps) {
  const { user } = useContext(AuthContext);

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

      <span className="habit-buttons-block">
        {user?.role === "Admin" && (
          <Button
            variant="delete"
            onClick={() => onDelete?.(habit.id)}
          >
            ❌
          </Button>
        )}
      </span>
    </div>
  );
};
