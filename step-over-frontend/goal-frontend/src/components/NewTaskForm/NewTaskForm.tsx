import { useState } from "react"
import { Button } from "../Button/Button"
import "./NewTaskForm.css"
import type { TaskToCreate } from "../../api/goal-tasks.types";

type NewTaskFormProps = {
  onAddTask: (taskToCreate: TaskToCreate) => void;
};

export default function NewTaskForm({ onAddTask }: NewTaskFormProps) {
  const [title, setTitle] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (title.trim() === "") {
      setError("Title can't be empty");
      return;
    }

    onAddTask({ title: title.trim() });
    setTitle("");
    setError("");
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setTitle(e.target.value);

    if (error && e.target.value.trim() !== "") {
      setError("");
    }
  };

  return (
    <div>
      <form className="add-task-form" onSubmit={handleSubmit}>
        <input
          type="text"
          value={title}
          placeholder="Procrastinate later, write it now"
          onChange={handleChange}
          className={error ? "error" : ""}
        />

        <Button type="submit">Add Task</Button>
      </form>
      {error && <p className="error-message">{error}</p>}
    </div>
  );
}
