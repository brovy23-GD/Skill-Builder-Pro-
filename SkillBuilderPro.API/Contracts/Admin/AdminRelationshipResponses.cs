namespace SkillBuilderPro.API.Contracts.Admin;

public sealed record AdminUserSummary(
    int UserId,
    string DisplayName,
    string Email,
    string ExpectedRole,
    bool HasExpectedRole);

public sealed record ParentAthleteResponse(
    AdminUserSummary Parent,
    AdminUserSummary Athlete,
    bool IsActive,
    DateTime CreatedAtUtc,
    int CreatedByUserId);

public sealed record TeamResponse(
    int Id,
    string Name,
    string Sport,
    string? Season,
    string? AgeGroup,
    string? Organization,
    bool IsActive,
    DateTime CreatedAtUtc,
    int CreatedByUserId);

public sealed record TeamCoachResponse(
    int TeamId,
    AdminUserSummary Coach,
    string TeamRole,
    bool IsActive,
    DateTime JoinedAtUtc);

public sealed record TeamAthleteResponse(
    int TeamId,
    AdminUserSummary Athlete,
    bool IsActive,
    DateTime JoinedAtUtc,
    DateTime? LeftAtUtc);
