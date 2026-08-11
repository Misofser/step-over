import { useToday, TodaySection } from "@/features/today";

export function TodayPage() {
  const { data, loading, toggleItem } = useToday();

  if (loading) return <div>Loading...</div>;
  if (!data) return <div>No data</div>;

  return (
    <div className="app-container">
      <h1>Focus</h1>
      <TodaySection
        title="Still avoiding"
        items={data.pending}
        onToggle={toggleItem}
        type="pending"
      />
      <TodaySection
        title="Crushed it"
        items={data.completed}
        onToggle={toggleItem}
        type="completed"
      />
    </div>
  );
}
