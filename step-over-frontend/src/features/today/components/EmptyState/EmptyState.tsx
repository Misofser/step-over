import type { SectionType } from "../../types/section.types";
import "./EmptyState.css";

type Props = {
  type: SectionType;
};

export function EmptyState({ type }: Props) {
  if (type === "pending") {
    return (
      <div className="empty-state">
        <h3>All done for today 🎉</h3>
        <p>Enjoy the rest of your time — you earned it</p>
      </div>
    );
  }

  return (
    <div className="empty-state">
      <h3>No completed items yet</h3>
      <p>Zero progress so far. Let's change that ⚔️</p>
    </div>
  );
}
