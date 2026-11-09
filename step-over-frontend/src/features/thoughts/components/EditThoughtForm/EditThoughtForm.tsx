import { useState } from "react";

import { Modal } from "@/components/Modal/Modal";
import type { Thought } from "../../types/thoughts.types";
import "./EditThoughtForm.css";

type EditThoughtFormProps = {
  thought: Thought;
  onClose: () => void;
  onSave: (content: string) => Promise<void>;
};

export function EditThoughtForm({
  thought,
  onClose,
  onSave,
}: EditThoughtFormProps) {
  const [content, setContent] = useState(thought.content);
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const hasChanges = content !== thought.content;

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
      await onSave(trimmedContent);
      onClose();
    } catch {
      setError("Could not save thought. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleRequestClose = () => {
    if (isSubmitting) return;

    if (hasChanges && !window.confirm("Discard unsaved changes?")) {
      return;
    }

    onClose();
  };

  const handleChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
    setContent(event.target.value);

    if (error) {
      setError("");
    }
  };

  return (
    <Modal title="Edit Thought" onClose={handleRequestClose}>
      <form className="edit-thought-form" onSubmit={handleSubmit}>
        <label htmlFor="edit-thought-content">Still on your mind?</label>
        <textarea
          id="edit-thought-content"
          name="content"
          value={content}
          onChange={handleChange}
          disabled={isSubmitting}
          className={error ? "error" : ""}
          aria-invalid={Boolean(error)}
          autoFocus
        />

        {error && <p className="error-message">{error}</p>}

        <div className="actions">
          <button
            type="submit"
            className="primary"
            disabled={isSubmitting || !hasChanges}
          >
            {isSubmitting ? "Saving..." : "Save"}
          </button>
          <button
            type="button"
            className="secondary"
            onClick={handleRequestClose}
            disabled={isSubmitting}
          >
            Cancel
          </button>
        </div>
      </form>
    </Modal>
  );
}
