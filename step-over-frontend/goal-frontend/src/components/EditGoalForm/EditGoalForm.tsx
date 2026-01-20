import { useState, useEffect } from "react"
import type { Goal } from "../../api/goals.types"
import { fetchGoal, updateGoal } from "../../api/goals"
import "./EditGoalForm.css"

interface EditGoalFormProps {
  goalId: number;
  onClose: () => void;
  onSave: (id: number, newTitle: string) => void;
}

export function EditGoalForm({ goalId, onClose, onSave }: EditGoalFormProps) {
  const [goal, setGoal] = useState<Goal | null>(null);
  const [title, setTitle] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadGoal() {
      const data = await fetchGoal(goalId);
      setGoal(data);
      setTitle(data.title);
    }
    loadGoal();
  }, [goalId]);

  const handleSave = async () => {
    if (!title.trim()) {
      setError("Title cannot be empty");
      return;
    }
    await updateGoal(goalId, { title: title.trim() });
    onSave(goalId, title.trim());
    onClose();
  };

  if (!goal) {
    return (
      <div className="modal-backdrop">
        <div className="modal">
          <p>Loading...</p>
        </div>
      </div>
    );
  };

  return (
    <div className="goal-form">
      <input
        value={title}
        onChange={e => {
          setTitle(e.target.value);
          setError("");
        }}
        onKeyDown={(e) => e.key === "Enter" && handleSave()}
        className={error ? "error" : ""}
      />

      {error && <p className="error-text">{error}</p>}

      <div className="actions">
        <button onClick={handleSave} className="primary">Save</button>
        <button onClick={onClose} className="secondary">Cancel</button>
      </div>
    </div>
  );
}
