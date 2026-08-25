using SkillBuilderPro.Core.Progression;

namespace SkillBuilderPro.Tests.Progression;

public sealed class ProgressionRulesTests
{
    [Theory]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(8, 3)]
    [InlineData(15, 4)]
    [InlineData(25, 5)]
    public void GetSkillLevel_WhenCompletionCrossesThreshold_ReturnsExpectedLevel(int completions, int expected) =>
        Assert.Equal(expected, ProgressionRules.GetSkillLevel(completions));

    [Theory]
    [InlineData(0, 1, 0)]
    [InlineData(1, 1, 33)]
    [InlineData(3, 2, 0)]
    [InlineData(7, 2, 80)]
    [InlineData(25, 5, 100)]
    [InlineData(-10, 1, 0)]
    public void GetSkillProgressPercent_ForBoundaries_ClampsAndCalculates(int completions, int level, int expected) =>
        Assert.Equal(expected, ProgressionRules.GetSkillProgressPercent(completions, level));

    [Fact]
    public void GetProgressionScore_WithMilestonesBreadthAndCappedStreak_SumsRules()
    {
        var score = ProgressionRules.GetProgressionScore(20, [1, 3, 5], 4, 15);
        Assert.Equal(69, score);
    }

    [Fact]
    public void GetProgressionScore_WithNegativeBonuses_DoesNotSubtractPoints()
    {
        var score = ProgressionRules.GetProgressionScore(4, [0, -2], 0, -5);
        Assert.Equal(4, score);
    }

    [Theory]
    [InlineData(25, 1, 3)]
    [InlineData(50, 1, 3)]
    [InlineData(50, 2, 4)]
    [InlineData(300, 3, 7)]
    [InlineData(300, 4, 8)]
    public void GetRank_WhenScoreOrBreadthVaries_RequiresBothThresholds(int score, int activeSkills, int expected) =>
        Assert.Equal(expected, ProgressionRules.GetRank(score, activeSkills));

    [Theory]
    [InlineData(10, 2, 0)]
    [InlineData(17, 2, 46)]
    [InlineData(300, 8, 100)]
    [InlineData(-5, 1, 0)]
    public void GetRankProgressPercent_ForRankRange_ReturnsClampedPercentage(int score, int rank, int expected) =>
        Assert.Equal(expected, ProgressionRules.GetRankProgressPercent(score, rank));

    [Fact]
    public void CalculateStreaks_WithNoCompletions_ReturnsZeroes() =>
        Assert.Equal((0, 0), ProgressionRules.CalculateStreaks([], new DateTime(2026, 8, 25)));

    [Fact]
    public void CalculateStreaks_WithDuplicatesAndConsecutiveDays_DeduplicatesDays()
    {
        var now = new DateTime(2026, 8, 25, 12, 0, 0, DateTimeKind.Utc);
        DateTime[] timestamps = [now.AddDays(-2), now.AddDays(-1), now.AddDays(-1).AddHours(2), now];
        Assert.Equal((3, 3), ProgressionRules.CalculateStreaks(timestamps, now));
    }

    [Fact]
    public void CalculateStreaks_WhenLastCompletionWasYesterday_KeepsCurrentStreak()
    {
        var now = new DateTime(2026, 8, 25, 12, 0, 0, DateTimeKind.Utc);
        Assert.Equal((2, 2), ProgressionRules.CalculateStreaks([now.AddDays(-2), now.AddDays(-1)], now));
    }

    [Fact]
    public void CalculateStreaks_WhenCurrentRunIsStale_PreservesLongestButResetsCurrent()
    {
        var now = new DateTime(2026, 8, 25, 12, 0, 0, DateTimeKind.Utc);
        Assert.Equal((0, 3), ProgressionRules.CalculateStreaks([now.AddDays(-10), now.AddDays(-9), now.AddDays(-8)], now));
    }
}
