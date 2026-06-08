import { useEffect, useRef } from "react";
import { Button } from "../../../../components/Button/Button";
import "./HabitMenu.css";

type Props = {
  open: boolean;
  onClose: () => void;
  onEditCompletion: () => void;
  onDelete?: () => void;
  isAdmin?: boolean;
};

export function HabitMenu({
  open,
  onClose,
  onEditCompletion,
  onDelete,
  isAdmin,
}: Props) {
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (
        ref.current && !ref.current.contains(e.target as Node)) {
        onClose();
      }
    }

    document.addEventListener("mousedown", handleClickOutside);

    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [onClose]);

  if (!open) return null;

  return (
    <div className="habit-menu" ref={ref}>
      <Button variant="edit" onClick={onEditCompletion}>
        📅 Manage completions
      </Button>

      {isAdmin && (
        <Button
					variant="delete"
					onClick={onDelete}
				>
          ❌ Delete habit
        </Button>
      )}
    </div>
  );
}
