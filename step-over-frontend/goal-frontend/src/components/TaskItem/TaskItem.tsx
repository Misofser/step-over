import { useContext } from "react";

import type { Task } from "../../api/goal-tasks.types";
import { Button } from "../Button/Button";
import { AuthContext } from "../../auth/AuthContext";
import "./TaskItem.css";

type TaskItemProps = {
  task: Task;
  onToggle: (task: Task) => void;
  onDelete: (id: number) => void;
  onEdit?: (task: Task) => void;
};

export function TaskItem({ task, onToggle, onDelete, onEdit }: TaskItemProps) {
  const { user } = useContext(AuthContext);

  return (
    <li key={task.id} className="task">
      <span className="task-checkbox">
        <input
          id={`task-${task.id}`}
          type="checkbox"
          checked={task.isCompleted}
          onChange={() => onToggle(task)}
        />
        <label htmlFor={`task-${task.id}`} className={`${task.isCompleted ? "completed" : ""}`}>  
          {task.title}
        </label>
      </span>
      <span className="task-buttons-block">
        <Button
          variant="edit"
          onClick={() => onEdit?.(task)}
        >
          ✏️
        </Button>
        {user?.role === "Admin" && (
          <Button
            variant="delete"
            onClick={() => onDelete?.(task.id)}
          >
            ❌
          </Button>
        )}
      </span>
    </li>
  );
}
