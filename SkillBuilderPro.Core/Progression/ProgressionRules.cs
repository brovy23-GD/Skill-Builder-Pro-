namespace SkillBuilderPro.Core.Progression;

public static class ProgressionRules
{
    public static readonly int[] SkillThresholds = [0, 3, 8, 15, 25];
    public static readonly string[] SkillLevelNames = ["Foundation", "Developing", "Competent", "Advanced", "Elite"];
    public static readonly int[] RankThresholds = [0, 10, 25, 50, 90, 140, 210, 300];
    public static readonly int[] RankBreadthRequirements = [0, 1, 1, 2, 2, 3, 3, 4];
    public static readonly string[] RankNames = ["Rookie", "Rising Star", "Competitor", "Playmaker", "All-Star", "Elite", "Champion", "Legend"];

    public static int GetSkillLevel(int completions)
    {
        for (var index = SkillThresholds.Length - 1; index >= 0; index--)
            if (completions >= SkillThresholds[index]) return index + 1;
        return 1;
    }

    public static int GetSkillProgressPercent(int completions, int level)
    {
        if (level >= SkillThresholds.Length) return 100;
        var current = SkillThresholds[level - 1];
        var next = SkillThresholds[level];
        return Math.Clamp((int)Math.Floor((completions - current) * 100d / (next - current)), 0, 100);
    }

    public static int GetProgressionScore(int completions, IEnumerable<int> skillLevels, int activeSkillCount, int longestStreak)
    {
        var milestonePoints = skillLevels.Sum(level => Math.Max(0, level - 1) * 5);
        var breadthPoints = Math.Max(0, activeSkillCount - 1) * 3;
        var streakPoints = Math.Min(Math.Max(0, longestStreak), 10);
        return completions + milestonePoints + breadthPoints + streakPoints;
    }

    public static int GetRank(int score, int activeSkillCount)
    {
        for (var index = RankThresholds.Length - 1; index >= 0; index--)
            if (score >= RankThresholds[index] && activeSkillCount >= RankBreadthRequirements[index]) return index + 1;
        return 1;
    }

    public static int GetRankProgressPercent(int score, int rank)
    {
        if (rank >= RankThresholds.Length) return 100;
        var current = RankThresholds[rank - 1];
        var next = RankThresholds[rank];
        return Math.Clamp((int)Math.Floor((score - current) * 100d / (next - current)), 0, 100);
    }

    public static (int Current, int Longest) CalculateStreaks(IEnumerable<DateTime> timestamps, DateTime utcNow)
    {
        var days = timestamps.Select(timestamp => timestamp.Date).Distinct().OrderBy(day => day).ToList();
        if (days.Count == 0) return (0, 0);

        var longest = 1;
        var run = 1;
        for (var index = 1; index < days.Count; index++)
        {
            run = days[index] == days[index - 1].AddDays(1) ? run + 1 : 1;
            longest = Math.Max(longest, run);
        }

        var last = days[^1];
        if (last < utcNow.Date.AddDays(-1)) return (0, longest);
        var current = 1;
        for (var index = days.Count - 1; index > 0 && days[index - 1] == days[index].AddDays(-1); index--)
            current++;
        return (current, longest);
    }
}
