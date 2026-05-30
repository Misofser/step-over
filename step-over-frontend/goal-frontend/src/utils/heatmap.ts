import type { GoalHeatmapDay } from "../api/goal-analytics/goal-heatmap.types";

export function getHeatmapIntensity(
  completed: number,
  total: number
): number {
  if (total === 0) return 0;

  const ratio = completed / total;

  if (ratio === 0) return 0;
  if (ratio < 0.4) return 1;
  if (ratio < 0.8) return 2;
  return 3;
}

const MS_IN_DAY = 1000 * 60 * 60 * 24;

const parseDate = (dateStr: string) => {
  const date = new Date(dateStr.slice(0, 10));
  return isNaN(date.getTime()) ? null : date;
};

export const getWeekdayIndex = (date: Date) => {
  return (date.getDay() + 6) % 7;
};

export const getStartOfWeek = (date: Date) => {
  const d = new Date(date);
  const diff = getWeekdayIndex(d);

  d.setDate(d.getDate() - diff);
  d.setHours(0, 0, 0, 0);

  return d;
};

export const getWeekIndex = (start: Date, current: Date) => {
  const diffDays = Math.floor(
    (current.getTime() - start.getTime()) / MS_IN_DAY
  );

  return Math.floor(diffDays / 7);
};

export const transformHeatmap = (days: GoalHeatmapDay[]) => {
  if (!days.length) return [];

  const parsedDates = days
    .map(d => parseDate(d.date))
    .filter((d): d is Date => d !== null);

  if (!parsedDates.length) return [];

  const minDate = new Date(
    Math.min(...parsedDates.map(d => d.getTime()))
  );

  const startDate = getStartOfWeek(minDate);

  return days.map(day => {
    const date = parseDate(day.date);

    if (!date) {
      return {
        ...day,
        weekday: 0,
        weekIndex: 0,
      };
    }

    return {
      ...day,
      weekday: getWeekdayIndex(date),
      weekIndex: getWeekIndex(startDate, date),
    };
  });
};
