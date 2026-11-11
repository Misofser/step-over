import { useState } from "react";

import type { Thought } from "../../types/thoughts.types";
import { ThoughtMenu } from "../ThoughtMenu/ThoughtMenu";
import "./ThoughtList.css";

type ThoughtListProps = {
  thoughts: Thought[];
  onEdit: (thought: Thought) => void;
};

export function ThoughtList({ thoughts, onEdit }: ThoughtListProps) {
  const [menuThoughtId, setMenuThoughtId] = useState<number | null>(null);

  if (thoughts.length === 0) {
    return (
      <section className="thought-list-empty">
        <h3>No thoughts yet</h3>
        <p>Capture a thought while it is still fresh.</p>
      </section>
    );
  }

  return (
    <section className="thought-list" aria-label="Thoughts inbox">
      {thoughts.map((thought) => (
        <article className="thought-card" key={thought.id}>
          <p>{thought.content}</p>
          <button
            type="button"
            className="thought-menu-button"
            aria-label="Open thought actions"
            aria-expanded={menuThoughtId === thought.id}
            onClick={() =>
              setMenuThoughtId((currentId) =>
                currentId === thought.id ? null : thought.id
              )
            }
          >
            ⋮
          </button>
          <ThoughtMenu
            open={menuThoughtId === thought.id}
            onClose={() => setMenuThoughtId(null)}
            onEdit={() => {
              setMenuThoughtId(null);
              onEdit(thought);
            }}
          />
        </article>
      ))}
    </section>
  );
}
