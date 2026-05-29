import { useState, useEffect } from "react"
import type { Task } from "../../api/goal-tasks.types"
import { fetchTask, updateTask } from "../../api/goal-tasks"
import "./EditTaskForm.css"

interface EditTaskFormProps {
  taskId: number;
  onClose: () => void;
  onSave: (id: number, newTitle: string) => void;
}

export function EditTaskForm({ taskId, onClose, onSave }: EditTaskFormProps) {
  const [task, setTask] = useState<Task | null>(null);
  const [title, setTitle] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadTask() {
      const data = await fetchTask(taskId);
      setTask(data);
      setTitle(data.title);
    }
    loadTask();
  }, [taskId]);

  const handleSave = async () => {
    if (!title.trim()) {
      setError("Title cannot be empty");
      return;
    }
    await updateTask(taskId, { title: title.trim() });
    onSave(taskId, title.trim());
    onClose();
  };

  if (!task) {
    return (
      <div className="modal-backdrop">
        <div className="modal">
          <p>Loading...</p>
        </div>
      </div>
    );
  };

  return (
    <div className="task-form">
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
