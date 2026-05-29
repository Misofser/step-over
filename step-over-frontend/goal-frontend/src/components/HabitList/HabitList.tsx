import type { Habit, HabitToCreate } from "../../api/habits.types";
import { HabitItem } from "../HabitItem/HabitItem";
import NewHabitForm from "../NewHabitForm/NewHabitForm";
import { toggleHabitCompletion } from "../../api/habits";
import "./HabitList.css";

type HabitListProps = {
  habits: Habit[];
  setHabits: React.Dispatch<React.SetStateAction<Habit[]>>
  addHabit: (habitToCreate: HabitToCreate) => void;
};

export function HabitList({ habits, setHabits, addHabit }: HabitListProps) {
  const handleToggle = async (habitId: number) => {
    const today = new Date().toISOString()

    try {
     await toggleHabitCompletion(habitId, today);
     setHabits(prev =>
        prev.map(h =>
          h.id === habitId ? { ...h, isCompletedToday: !h.isCompletedToday } : h
        )
      );
    } catch (e) {
      alert("Error toggling habit");
    }
  };

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
              <HabitItem habit={habit} onToggle={handleToggle} />
            </li>
          ))}
        </ul>
      )}
    </section>
  );
};
