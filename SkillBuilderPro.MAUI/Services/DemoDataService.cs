using SkillBuilderPro.MAUI.Models;
using CoreDrill = SkillBuilderPro.Core.Models.Drill;

namespace SkillBuilderPro.MAUI.Services;

/// <summary>Curated product-demo state. Authenticated flows never read this source.</summary>
public static class DemoDataService
{
    public const string DisplayName = "Aubrey Rovy";
    public const string Sport = "Softball";
    public const string Rank = "Competitor";
    public const string LockerNumber = "3";
    public const int UnreadNotifications = 2;

    public static Progression Progression => new(0, Rank, 3, 680, 68, "Contender", 1000, 320, 24, 5, 6, 9, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

    public static Goal[] Goals =>
    [
        new(1, 0, "Skill", Sport, "Hitting", "Contact", "Improve Batting Contact", "Build repeatable barrel contact against game-speed pitching.", 100, "85% quality contact", 25, 25, "Active", false, false, DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddDays(-14), DateTime.UtcNow, null, null),
        new(2, 0, "Training", Sport, "Hitting", "Timing", "Complete 4 Hitting Sessions", "Complete focused timing and bat-path sessions this week.", 4, "4 sessions", 2, 50, "Active", false, false, DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(-8), DateTime.UtcNow, null, null),
        new(3, 0, "Repetitions", Sport, "Fielding", "Infield", "Fielding Repetition Goal", "Clean footwork and transfer through 200 quality repetitions.", 200, "200 reps", 150, 75, "Active", false, false, DateTime.UtcNow.AddDays(18), DateTime.UtcNow.AddDays(-12), DateTime.UtcNow, null, null),
        new(4, 0, "Consistency", Sport, "Training", "Weekly", "Four-Week Consistency Block", "Maintain the training plan without missing a scheduled session.", 20, "20 sessions", 18, 90, "Active", false, false, DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(-23), DateTime.UtcNow, null, null),
        new(5, 0, "Milestone", Sport, "Hitting", "Bat Speed", "Foundation Bat-Speed Block", "Completed the first measured bat-speed development block.", 12, "12 sessions", 12, 100, "Completed", true, false, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-28), DateTime.UtcNow.AddDays(-3), DateTime.UtcNow.AddDays(-3), null)
    ];

    public static AthleteAssignment[] Assignments =>
    [
        new(1, new DrillSummary(1001, "Timing & Bat Path", Sport, "Hitting"), new UserSummary(0, "Skill Builder Pro"), null, DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1), "Hitting Development · 3 drills · 25 minutes", "Active", true, DateTime.UtcNow.AddHours(-4), "Ready", null, null, null, null)
    ];

    public static NotificationItem[] Notifications =>
    [
        new(1, "Assignment", "Today's softball training is ready", "Timing & Bat Path includes three focused drills.", null, null, false, null, DateTime.UtcNow.AddHours(-2), "//Training"),
        new(2, "Milestone", "Consistency milestone", "Your six-day training streak is active.", null, null, false, null, DateTime.UtcNow.AddDays(-1), "//Trophy")
    ];

    public static CoreDrill[] Drills =>
    [
        new() { Id = 100201, Name = "Side-to-Side Shuffle and Throw Infield Drill", Sport = Sport, Category = "Fielding", SubCategory = "Fly Balls", Description = "Build efficient lateral movement and accurate throws.", Difficulty = 1, Duration = "10:00", VideoUrl = "https://www.youtube.com/watch?v=rFEgj683qh0" },
        new() { Id = 100202, Name = "Two Infield Drills for Quick, Efficient Movement", Sport = Sport, Category = "Fielding", SubCategory = "Infield", Description = "Develop quick feet and efficient infield movement patterns.", Difficulty = 2, Duration = "10:00", VideoUrl = "https://www.youtube.com/watch?v=w7-41ueexqo" },
        new() { Id = 100203, Name = "Two Outfield Drills for Explosive Movement", Sport = Sport, Category = "Fielding", SubCategory = "Outfield", Description = "Train explosive first steps and confident outfield tracking.", Difficulty = 3, Duration = "10:00", VideoUrl = "https://www.youtube.com/watch?v=wYmTKx-8Sdk" }
    ];

    public static TrophyRoom Trophy => new(
        Progression,
        [new RankHistory(1, "Rookie", DateTime.UtcNow.AddDays(-90), 100, 4, 1), new RankHistory(2, "Contender", DateTime.UtcNow.AddDays(-52), 300, 11, 2), new RankHistory(3, Rank, DateTime.UtcNow.AddDays(-21), 500, 18, 4)],
        [new SkillMilestone(Sport, "Hitting", "Timing", 2, "Timing Builder", DateTime.UtcNow.AddDays(-3), 12, 4.3), new SkillMilestone(Sport, "Fielding", "Infield", 2, "Reliable Defender", DateTime.UtcNow.AddDays(-11), 9, 4.1), new SkillMilestone(Sport, "Hitting", "Contact", 1, "Contact Foundation", DateTime.UtcNow.AddDays(-27), 6, 3.9)],
        [
            new Achievement("STREAK_5", "5-Day Training Streak", "Completed qualifying training on five consecutive days.", "Consistency", "Silver", true, DateTime.UtcNow.AddDays(-1), 1),
            new Achievement("SESSIONS_10", "Ten Sessions Complete", "Completed ten focused training sessions.", "Training", "Bronze", true, DateTime.UtcNow.AddDays(-8), 2),
            new Achievement("HITTING_10", "Hitting Foundation", "Completed ten hitting development sessions.", "Skill", "Silver", true, DateTime.UtcNow.AddDays(-12), 3),
            new Achievement("GOAL_1", "First Goal Achieved", "Completed the first tracked athlete goal.", "Goals", "Bronze", true, DateTime.UtcNow.AddDays(-18), 4),
            new Achievement("STREAK_10", "10-Day Training Streak", "Complete qualifying training on ten consecutive days.", "Consistency", "Gold", false, null, 5),
            new Achievement("SESSIONS_25", "Twenty-Five Sessions", "Complete twenty-five focused training sessions.", "Training", "Gold", false, null, 6),
            new Achievement("RANK_4", "Advanced Competitor", "Reach the next athlete rank.", "Progression", "Gold", false, null, 7)
        ]);
}
