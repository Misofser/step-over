import { useState } from "react";
import type { HeatmapCell } from "../../api/goal-analytics/goal-heatmap.types";
import { GoalHeatmapTooltip } from "./GoalHeatmapTooltip/GoalHeatmapTooltip";
import { getHeatmapIntensity } from "../../utils/heatmap";
import { getLegendLabel } from "../../utils/heatmap.legend";

import './GoalHeatmap.css';

type GoalHeatmapProps = {
  days: HeatmapCell[];
};

type HoverState = {
  x: number;
  y: number;
  day: HeatmapCell | null;
};

export function GoalHeatmap({ days }: GoalHeatmapProps) {
  const weekDays = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

  const [hover, setHover] = useState<HoverState>({
    x: 0,
    y: 0,
    day: null,
  });

  return (
    <section className="goal-heatmap-card">
      <div className="goal-heatmap-header">
        <div className="goal-heatmap-title">Habit consistency</div>

        <p>
          Completed habits over the last {days.length} days
        </p>
      </div>

      <div className="goal-heatmap-layout">
        <div className="goal-heatmap-block">
          <div className="heatmap-labels">
            {weekDays.map(day => (
              <span key={day}>{day}</span>
            ))}
          </div>

          
          <div className="goal-heatmap-wrapper">
            <div className="goal-heatmap">
              {days.map(day => {
                const intensity = getHeatmapIntensity(
                  day.completedHabits,
                  day.totalHabits
                );

                return (
                  <div
                    key={day.date}
                    className={`heatmap-cell intensity-${intensity}`}
                    style={{
                      gridRow: day.weekday + 1,
                      gridColumn: day.weekIndex + 1,
                    }}
                    onMouseEnter={(e) =>
                      setHover({
                        x: e.clientX,
                        y: e.clientY,
                        day,
                      })
                    }

                    onMouseLeave={() =>
                      setHover(prev => ({ ...prev, day: null }))
                    }
                  />
                );
              })}
            </div>
          </div>
        </div>

        <div className="heatmap-legend">
          <div className="legend-scale">
            {[0, 1, 2, 3].map(i => (
              <div
                key={i}
                className="legend-item"
              >
                <div className={`heatmap-cell intensity-${i}`} />

                <span className="legend-label">
                  {getLegendLabel(i)}
                </span>
              </div>
            ))}
          </div>
        </div>
      </div>

      <GoalHeatmapTooltip
        x={hover.x}
        y={hover.y}
        visible={!!hover.day}
        date={hover.day?.date || ""}
        completed={hover.day?.completedHabits || 0}
        total={hover.day?.totalHabits || 0}
      />
    </section>
  );
}
