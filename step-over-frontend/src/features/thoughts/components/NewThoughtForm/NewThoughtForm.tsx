import { useState } from "react";

import type { ThoughtToCreate } from "../../types/thoughts.types";
import "./NewThoughtForm.css";

type NewThoughtFormProps = {
  onAddThought: (thoughtToCreate: ThoughtToCreate) => Promise<void>;
  onClose: () => void;
};

export function NewThoughtForm({ onAddThought, onClose }: NewThoughtFormProps) {
  const [content, setContent] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    const trimmedContent = content.trim();

    if (!trimmedContent) {
      setError("Thought can't be empty");
      return;
    }

    setIsSubmitting(true);
    setError("");

    try {
      await onAddThought({ content: trimmedContent });
      onClose();
    } catch {
      setError("Could not save thought. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
    setContent(event.target.value);

    if (error && event.target.value.trim()) {
      setError("");
    }
  };

  return (
    <form className="new-thought-form" onSubmit={handleSubmit}>
      <label htmlFor="thought-content">Anything on your mind?</label>
      <textarea
        id="thought-content"
        name="content"
        value={content}
        placeholder="Capture a thought..."
        onChange={handleChange}
        disabled={isSubmitting}
        className={error ? "error" : ""}
        aria-invalid={Boolean(error)}
        autoFocus
      />

      {error && <p className="error-message">{error}</p>}

      <div className="actions">
        <button type="submit" className="primary" disabled={isSubmitting}>
          {isSubmitting ? "Creating..." : "Create"}
        </button>
        <button
          type="button"
          className="secondary"
          onClick={onClose}
          disabled={isSubmitting}
        >
          Cancel
        </button>
      </div>
    </form>
  );
}
