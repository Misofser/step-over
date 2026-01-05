import { useState } from "react";

import "./NewGoalForm.css"

type NewGoalFormProps = {
  onAddGoal: (title: string) => void;
};

export default function NewGoalForm({ onAddGoal }: NewGoalFormProps) {
  const [title, setTitle] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (title.trim() === "") {
      setError("Title can't be empty");
      return;
    }

    onAddGoal(title.trim());
    setTitle("");
    setError("");
  };

  return (
    <div>
      <form className="add-goal-form" onSubmit={handleSubmit}>
        <input
          type="text"
          value={title}
          placeholder="What’s your next audacious goal?"
          onChange={e => setTitle(e.target.value)}
          className={error ? "error" : ""}
        />
        <button type="submit">Add Goal</button>
      </form>
      {error && <p className="error-message">{error}</p>}
    </div>
  );
}
