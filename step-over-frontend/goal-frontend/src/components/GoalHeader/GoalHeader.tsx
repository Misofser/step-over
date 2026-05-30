import { useContext, useEffect, useRef, useState } from "react";
import type { Goal } from "../../api/goals.types";
import { GoalTypeBadge } from "../GoalTypeBadge/GoalTypeBadge";
import { Button } from "../Button/Button";
import { AuthContext } from "../../auth/AuthContext";
import "./GoalHeader.css"

export function GoalHeader({
  goal,
  onRename,
  onDelete
}: { 
  goal: Goal; 
  onRename: (goal: Goal) => void;
  onDelete: () => void; 
}) {
  const { user } = useContext(AuthContext);

  const [open, setOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setOpen(false);
      }
    }

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  return (
    <header  className="title-block">
      <span className="goal-label">Goal</span>
      <h1 className={`${goal.isCompleted ? "completed" : ""}`}>{goal.title}</h1>

      <div className="menu-wrapper" ref={menuRef}>
        <button
          className="menu-button"
          onClick={() => setOpen(prev => !prev)}
        >
          ⋮
        </button>

        {open && (
          <div className="dropdown-menu">
            <Button
              variant="edit"
              onClick={() => onRename(goal)}
            >
              ✏️ Rename goal
            </Button>
            {user?.role === "Admin" && (
            <Button
              variant="delete"
              onClick={() => onDelete()}
            >
              ❌ Delete goal
            </Button>
        )}
          </div>
        )}
      </div>
      <GoalTypeBadge type={goal.type} />
    </header>
  );
}
