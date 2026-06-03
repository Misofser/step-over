import type { Habit, HabitToCreate } from "../../api/habits.types";
import { HabitItem } from "../HabitItem/HabitItem";
import NewHabitForm from "../NewHabitForm/NewHabitForm";
// import { toggleHabitCompletion, deleteHabit } from "../../api/habits";
import "./HabitList.css";

type HabitListProps = {
  habits: Habit[];
  addHabit: (habitToCreate: HabitToCreate) => void;
  onToggle: (id: number) => void;
  onDelete: (id: number) => void;
};

export function HabitList({ habits, onToggle, onDelete, addHabit }: HabitListProps) {
  return (
    <section className="habits-section">
      <h2>Habits</h2>
      <p className="section-subtitle">Small actions, big change</p>
      <NewHabitForm onAddHabit={addHabit}/>

      {habits.length === 0 ? (
        <p>No habits yet</p>
      ) : (
        <ul className="habit-list">
          {habits.map(habit => (
            <li key={habit.id}>
              <HabitItem
                habit={habit}
                onToggle={onToggle}
                onDelete={onDelete}
              />
            </li>
          ))}
        </ul>
      )}
    </section>
  );
};
