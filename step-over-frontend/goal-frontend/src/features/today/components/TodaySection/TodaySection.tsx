import type { TodayItem } from "../../types/today.types";
import { TodayItemRow } from "../TodayItemRow/TodayItemRow";
import { EmptyState } from "../EmptyState/EmptyState";
import type { SectionType } from "../../types/section.types";
import "./TodaySection.css";

type Props = {
  title: string;
  items: TodayItem[];
  onToggle: (item: TodayItem) => void;
  type: SectionType;
};

export function TodaySection({ title, items, onToggle, type }: Props) {
  return (
    <section className="today-section">
      <h2 className="today-section-title">{title}</h2>

      {items.length === 0 ? (
        <EmptyState type={type} />
      ) : (
        <ul className="today-list">
          {items.map(item => (
            <li key={`${item.type}-${item.entityId}`}>
              <TodayItemRow
                item={item}
                onToggle={onToggle}
              />
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
