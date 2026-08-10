import { formatHeatmapDate, formatHeatmapProgress } from "../../../utils/heatmap.tooltip";
import './GoalHeatmapTooltip.css';

type Props = {
  x: number;
  y: number;
  visible: boolean;
  date: string;
  completed: number;
  total: number;
};

export function GoalHeatmapTooltip({
  x,
  y,
  visible,
  date,
  completed,
  total,
}: Props) {
  if (!visible) return null;

  return (
    <div
      className="heatmap-tooltip"
      style={{
        left: x + 10,
        top: y + 10,
      }}
    >
      <div className="tooltip-date">
        {formatHeatmapDate(date)}
      </div>

      <div className="tooltip-progress">
        {formatHeatmapProgress(completed, total)}
      </div>
    </div>
  );
}
