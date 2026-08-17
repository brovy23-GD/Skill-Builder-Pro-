using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Data;

public static class AchievementDefinitionInitializer
{
    private sealed record Definition(string Code, string Name, string Description, string Category, string Tier, int SortOrder);
    private static readonly Definition[] Definitions =
    [
        new(AchievementCodes.FirstCompletion, "First Step", "Complete your first qualifying assignment.", AchievementCategories.Training, AchievementTiers.Bronze, 10),
        new(AchievementCodes.FirstSkillDeveloping, "Developing Skill", "Reach Developing in any skill.", AchievementCategories.Skill, AchievementTiers.Bronze, 20),
        new(AchievementCodes.RankRisingStar, "Rising Star", "Earn the Rising Star overall rank.", AchievementCategories.Rank, AchievementTiers.Silver, 30),
        new(AchievementCodes.TenQualifyingCompletions, "Training 10", "Complete ten qualifying assignments.", AchievementCategories.Training, AchievementTiers.Silver, 40),
        new(AchievementCodes.ThreeDayStreak, "Three-Day Streak", "Train on three consecutive UTC days.", AchievementCategories.Consistency, AchievementTiers.Silver, 50)
    ];

    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("AchievementDefinitionInitializer");
        var existing = await db.AchievementDefinitions.ToDictionaryAsync(x => x.Code, cancellationToken);
        foreach (var source in Definitions)
        {
            if (!existing.TryGetValue(source.Code, out var definition))
            {
                definition = new AchievementDefinition { Code = source.Code, CreatedAtUtc = DateTime.UtcNow };
                db.AchievementDefinitions.Add(definition);
            }
            definition.Name = source.Name; definition.Description = source.Description; definition.Category = source.Category;
            definition.Tier = source.Tier; definition.SortOrder = source.SortOrder; definition.IsActive = true;
        }
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Achievement definition initialization synchronized {DefinitionCount} definitions.", Definitions.Length);
    }
}
