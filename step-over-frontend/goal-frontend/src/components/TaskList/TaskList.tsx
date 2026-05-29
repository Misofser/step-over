import { useState } from "react";
import type { Task, TaskToCreate } from "../../api/goal-tasks.types";
import { Modal } from "../Modal/Modal";
import { EditTaskForm } from "../EditTaskForm/EditTaskForm";
import { TaskItem } from "../TaskItem/TaskItem";
import { updateTaskCompletion, deleteTask } from "../../api/goal-tasks";
import NewTaskForm from "../NewTaskForm/NewTaskForm";
import "./TaskList.css";

type TaskListProps = {
  tasks: Task[]
  setTasks: React.Dispatch<React.SetStateAction<Task[]>>
  addTask: (taskToCreate: TaskToCreate) => void;
}

export function TaskList({ tasks, setTasks, addTask }: TaskListProps) {
  const [editingTaskId, setEditingTaskId] = useState<number | null>(null);

  const handleToggle = async (task: Task) => {
    try {
      await updateTaskCompletion(task.id, !task.isCompleted);
      setTasks(prev =>
        prev.map(t =>
          t.id === task.id ? { ...t, isCompleted: !t.isCompleted } : t
        )
      );
    } catch (e) {
      alert("Could not update task completion")
    }
  };

  const handleDeleteTask = async (id: number) => {
    try {
      await deleteTask(id);
      setTasks(prev => prev.filter(task => task.id !== id));
    } catch (e) {
      alert("Could not delete task");
    }
  };

  const handleSavedTask = (id: number, newTitle: string) => {
    setTasks(prev =>
      prev.map(t =>
        t.id === id ? { ...t, title: newTitle } : t
      )
    );
  };

  return (
    <div>
      <section className="goal-tasks-section">
        <h2>Tasks</h2>
        <p className="section-subtitle">Focus and finish</p>
        <NewTaskForm onAddTask={addTask} />
        {tasks.length === 0 ? (
          <p>No tasks yet</p>
        ) : (
          <ul>
            {tasks.map(task => (
              <TaskItem
                key={task.id}
                task={task}
                onToggle={handleToggle}
                onEdit={task => setEditingTaskId(task.id)}
                onDelete={handleDeleteTask}
              />
            ))}
          </ul>
        )}
      </section>
      {editingTaskId && (
        <Modal title="Edit Task">
          <EditTaskForm
            taskId={editingTaskId}
            onClose={() => setEditingTaskId(null)}
            onSave={handleSavedTask}
          />
        </Modal>
        )}
    </div>
  );
}
