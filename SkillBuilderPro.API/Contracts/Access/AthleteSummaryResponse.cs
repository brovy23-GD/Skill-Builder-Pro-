namespace SkillBuilderPro.API.Contracts.Access;

public sealed record AthleteSummaryResponse(int UserId, string DisplayName, string Sport, string ExperienceLevel);
public sealed record CoachTeamResponse(int TeamId, string Name, string Sport, string? Season, string? AgeGroup, string? Organization);
