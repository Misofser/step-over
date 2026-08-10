import { useState } from "react"
import { Button } from "../../../../components/Button/Button"
import "./NewHabitForm.css"
import type { HabitToCreate, HabitFrequency } from "../../types/habits.types";

type NewHabitFormProps = {
  onAddHabit: (habitToCreate: HabitToCreate) => void;
};

export default function NewHabitForm({ onAddHabit }: NewHabitFormProps) {
  const [title, setTitle] = useState("");
  const [frequency, setFrequency] = useState<HabitFrequency>("Daily");
  const [error, setError] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (title.trim() === "") {
      setError("Title can't be empty");
      return;
    }

    onAddHabit({ title: title.trim(), frequency });

    setTitle("");
    setFrequency("Daily");
    setError("");
  };

  return (
    <div>
      <form className="add-habit-form" onSubmit={handleSubmit}>
        <input
          id="habit-title"
          name="title"
          type="text"
          value={title}
          placeholder="Build a habit…"
          onChange={(e) => {
            setTitle(e.target.value);
            if (error && e.target.value.trim() !== "") setError("");
          }}
          className={error ? "error" : ""}
        />

        <div className="frequency-selector">
          {(["Daily", "Weekly", "Monthly"] as HabitFrequency[]).map((f) => (
            <button
              key={f}
              type="button"
              className={`frequency-chip ${
                frequency === f ? "active" : ""
              }`}
              onClick={() => setFrequency(f)}
            >
              {f}
            </button>
          ))}
        </div>

        <Button type="submit">Add Habit</Button>
      </form>
      {error && <p className="error-message">{error}</p>}
    </div>
  );
}
