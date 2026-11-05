import type { Thought } from "../../types/thoughts.types";
import "./ThoughtList.css";

type ThoughtListProps = {
  thoughts: Thought[];
};

export function ThoughtList({ thoughts }: ThoughtListProps) {
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
        </article>
      ))}
    </section>
  );
}
