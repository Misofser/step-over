import { useEffect, useRef } from "react";

import { Button } from "@/components/Button/Button";
import "./ThoughtMenu.css";

type ThoughtMenuProps = {
  open: boolean;
  onClose: () => void;
  onEdit: () => void;
};

export function ThoughtMenu({ open, onClose, onEdit }: ThoughtMenuProps) {
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!open) return;

    function handleClickOutside(event: MouseEvent) {
      if (ref.current && !ref.current.contains(event.target as Node)) {
        onClose();
      }
    }

    document.addEventListener("mousedown", handleClickOutside);

    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [open, onClose]);

  if (!open) return null;

  return (
    <div className="thought-menu" ref={ref}>
      <Button variant="edit" onClick={onEdit}>
        ✏️ Edit
      </Button>
    </div>
  );
}
