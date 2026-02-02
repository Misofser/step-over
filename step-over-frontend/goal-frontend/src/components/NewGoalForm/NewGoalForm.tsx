import { useState } from "react"
import { Button } from "../Button/Button"
import "./NewGoalForm.css"
import type { GoalToCreate } from "../../api/goals.types";

type NewGoalFormProps = {
  onAddGoal: (goalToCreate: GoalToCreate) => void;
};

export default function NewGoalForm({ onAddGoal }: NewGoalFormProps) {
  const [title, setTitle] = useState("");
  const [error, setError] = useState("");
  const [type, setType] = useState<GoalToCreate["type"] | "">("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (title.trim() === "") {
      setError("Title can't be empty");
      return;
    }

    if (!type) {
      setError("Type is required");
      return;
    }

    onAddGoal({ title: title.trim(), type });
    setTitle("");
    setError("");
    setType("");
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setTitle(e.target.value);

    if (error && e.target.value.trim() !== "") {
      setError("");
    }
  };

  return (
    <div>
      <form className="add-goal-form" onSubmit={handleSubmit}>
        <input
          type="text"
          value={title}
          placeholder="What’s your next audacious goal?"
          onChange={handleChange}
          className={error ? "error" : ""}
        />

        <div className="select-wrapper">
          <select
            value={type}
            onChange={(e) => setType(e.target.value as GoalToCreate["type"])}
            className={`goal-type-select ${error ? "error" : ""}`}
          >
            <option value="">Select type</option>
            <option value="Project">Project</option>
            <option value="Process">Process</option>
          </select>
        </div>
        <Button type="submit">Add Goal</Button>
      </form>
      {error && <p className="error-message">{error}</p>}
    </div>
  );
}
