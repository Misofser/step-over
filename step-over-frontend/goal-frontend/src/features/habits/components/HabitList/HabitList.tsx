import { useState, useContext } from "react";
import type { Habit, HabitToCreate } from "../../types/habits.types";
import { HabitItem } from "../HabitItem/HabitItem";
import { HabitMenu } from "../HabitMenu/HabitMenu";
import { EditHabitCompletionModal } from "../EditHabitCompletionModal/EditHabitCompletionModal";
import NewHabitForm from "../NewHabitForm/NewHabitForm";
import { AuthContext } from "../../../../auth/AuthContext";

import "./HabitList.css";

type HabitListProps = {
  habits: Habit[];
  addHabit: (habitToCreate: HabitToCreate) => void;
  onToggle: (id: number, date: string) => Promise<void>;
  onDelete: (id: number) => void;
};

export function HabitList({  habits, onToggle, onDelete, addHabit }: HabitListProps) {
  const [menuHabitId, setMenuHabitId] = useState<number | null>(null);
  const [editHabit, setEditHabit] = useState<Habit | null>(null);
  const { user } = useContext(AuthContext);

  return (
    <section className="habits-section">
      <h2>Habits</h2>
      <p className="section-subtitle">Small actions, big change</p>
      <NewHabitForm onAddHabit={addHabit} />

      {habits.length === 0 ? (
        <p>No habits yet</p>
      ) : (
        <ul className="habit-list">
          {habits.map((habit) => (
            <li className="habit-line" key={habit.id}>
              <HabitItem
                habit={habit}
                onToggle={onToggle}
                onOpenMenu={(id) => setMenuHabitId(id)}
              />
              <HabitMenu
                open={menuHabitId === habit.id}
                onClose={() => setMenuHabitId(null)}
                onEditCompletion={() => {
                  setEditHabit(habit);
                  setMenuHabitId(null);
                }}
                onDelete={() => {
                  onDelete?.(habit.id);
                  setMenuHabitId(null);
                }}
                isAdmin={user?.role === "Admin"}
              />
            </li>
          ))}
        </ul>
      )}

      {editHabit && (
        <EditHabitCompletionModal
          habit={editHabit}
          onClose={() => setEditHabit(null)}
          onSave={async (date) => {
            await onToggle(
              editHabit.id,
              date
            );
          }}
        />
      )}
    </section>
  );
}
