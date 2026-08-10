export const getLegendLabel = (i: number) => {
  switch (i) {
    case 0:
      return "🛌 rest day? sure. let's call it that.";
    case 1:
      return "🙂 almost productive, but not quite";
    case 2:
      return "😎 functioning at acceptable levels";
    case 3:
      return "🐺 beast mode unlocked (no one stopped it)";
    default:
      return "";
  }
};
