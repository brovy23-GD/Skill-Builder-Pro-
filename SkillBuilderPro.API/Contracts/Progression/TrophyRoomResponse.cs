using SkillBuilderPro.Core.Interfaces;

namespace SkillBuilderPro.API.Contracts.Progression;

public sealed record TrophyRoomResponse(AthleteProgressionResponse CurrentProgression, IReadOnlyCollection<RankHistoryView> RankHistory, IReadOnlyCollection<SkillLevelHistoryView> SkillMilestones, IReadOnlyCollection<AchievementView> Achievements);
public static class TrophyRoomMapper
{
    public static TrophyRoomResponse ToResponse(this TrophyRoomView view) => new(view.CurrentProgression.ToResponse(), view.RankHistory, view.SkillMilestones, view.Achievements);
}
