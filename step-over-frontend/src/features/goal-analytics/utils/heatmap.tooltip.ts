export const formatHeatmapDate = (dateStr: string) => {
  const date = new Date(dateStr);

  return new Intl.DateTimeFormat("en-GB", {
    weekday: "short",
    day: "2-digit",
    month: "short",
  }).format(date);
};

export const formatHeatmapProgress = (
  completed: number,
  total: number
) => {
  const percent = total === 0 ? 0 : Math.round((completed / total) * 100);
  return `${completed}/${total} (${percent}%)`;
};
