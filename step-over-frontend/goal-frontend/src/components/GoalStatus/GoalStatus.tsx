import './GoalStatus.css'

type GoalStatusProps = {
  isCompleted: boolean;
  onToggleCompletion: () => void;
};

export function GoalStatus({
  isCompleted,
  onToggleCompletion,
}: GoalStatusProps) {
  return (
    <div className="goal-status">
      {isCompleted ? (
        <>
          <span className="completed-badge">
            ✔ Goal completed
          </span>
          <button
            className="reopen-button"
            onClick={onToggleCompletion}
          >
            ⟳ Reopen goal
          </button>
        </>
      ) : (
        <button
          className="complete-button"
          onClick={onToggleCompletion}
        >
          Mark as completed
        </button>
      )}
    </div>
  );
}
