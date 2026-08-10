import { useState, useEffect } from "react";
import { Modal } from "../../../../components/Modal/Modal";
import "./EditHabitCompletionModal.css";
import { useHabitCompletionStatus } from "../../hooks/useHabitCompletionStatus";
import type { Habit } from "../../types/habits.types";

type EditHabitCompletionModalProps = {
  habit: Habit
  onClose: () => void;
  onSave: (date: string) => Promise<void>;
};

export function EditHabitCompletionModal({
  habit,
  onClose,
  onSave,
}: EditHabitCompletionModalProps) {
  const today = new Date().toISOString().split("T")[0];
  const [date, setDate] = useState(today);
  const {
    isCompleted,
    setIsCompleted,
    loading,
    loadStatus,
  } = useHabitCompletionStatus(habit.id);

  useEffect(() => {
    if (date === today) {
      setIsCompleted(habit.isCompletedToday);
      return;
    }

    loadStatus(date);
  }, [date, habit.isCompletedToday, loadStatus]);

  const handleSubmit = async () => {
    await onSave(date);
    onClose();
  };

  return (
    <Modal title="Edit completion history" onClose={onClose}>
      <div className="modal-context">
        <span className="label">Habit:</span>
        <span className="value">{habit.title}</span>
      </div>

      <input
        type="date"
        value={date}
        max={today}
        onChange={(e) => setDate(e.target.value)}
      />

      <div>
        <p>
          Current status:
          {" "}
          <strong>
            {loading ? "Loading..." : isCompleted ? "Completed" : "Not completed"}
          </strong>
        </p>
      </div>

      <div className="actions">
        <button onClick={handleSubmit} className="primary">
          {isCompleted ? "Remove completion" : "Mark completed"}
        </button>
        <button onClick={onClose} className="secondary">Cancel</button>
      </div>
    </Modal>
  );
}
