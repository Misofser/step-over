import './GoalProgress.css';

type GoalProgressProps = {
  completedTasks: number;
  totalTasks: number;
};

export function GoalProgress({
  completedTasks,
  totalTasks,
}: GoalProgressProps) {
  const progress =
    totalTasks === 0
      ? 0
      : Math.round((completedTasks / totalTasks) * 100);

  return (
    <div className="goal-progress">
      <div>
        {completedTasks} / {totalTasks} tasks completed
      </div>

      <div className="progress-bar-wrapper">
        <div
          className="progress-bar-fill"
          style={{ width: `${progress}%` }}
        />
      </div>
    </div>
  );
}
