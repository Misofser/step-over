import { useState } from "react";

import { Button } from "@/components/Button/Button";
import { Modal } from "@/components/Modal/Modal";
import type { Thought } from "@/features/thoughts";
import {
  EditThoughtForm,
  NewThoughtForm,
  ThoughtList,
  useThoughts,
} from "@/features/thoughts";
import "./ThoughtsPage.css";

export function ThoughtsPage() {
  const [isCreating, setIsCreating] = useState(false);
  const [editingThought, setEditingThought] = useState<Thought | null>(null);
  const { thoughts, addThought, updateThought, loading, error } = useThoughts();

  const inboxThoughts = thoughts.filter((thought) => thought.status === "Inbox");

  return (
    <div className="app-container">
      <h1>Thoughts</h1>
      <div className="thoughts-page-actions">
        <Button onClick={() => setIsCreating(true)}>Add Thought</Button>
      </div>

      {loading && <p>Loading...</p>}
      {!loading && error && <p>{error}</p>}
      {!loading && !error && (
        <ThoughtList thoughts={inboxThoughts} onEdit={setEditingThought} />
      )}

      {isCreating && (
        <Modal title="Add New Thought" onClose={() => setIsCreating(false)}>
          <NewThoughtForm
            onAddThought={addThought}
            onClose={() => setIsCreating(false)}
          />
        </Modal>
      )}

      {editingThought && (
        <EditThoughtForm
          thought={editingThought}
          onClose={() => setEditingThought(null)}
          onSave={(content) => updateThought(editingThought.id, { content })}
        />
      )}
    </div>
  );
}
