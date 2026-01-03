import { useState } from 'react';

import "../styles/forms.css"

type NewGoalFormProps = {
  onAddGoal: (title: string) => void;
};

export default function NewGoalForm({ onAddGoal }: NewGoalFormProps) {
  const [title, setTitle] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (title.trim() === '') return;
    onAddGoal(title);
    setTitle('');
  };

  return (
    <form className="add-goal-form" onSubmit={handleSubmit}>
      <input
        type="text"
        value={title}
        placeholder="What’s your next audacious goal?"
        onChange={e => setTitle(e.target.value)}
      />
      <button type="submit">Add Goal</button>
    </form>
  );
}
