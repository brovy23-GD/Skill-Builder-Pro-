namespace SkillBuilderPro.API.Contracts.Authentication;

public sealed record CurrentUserResponse(
    int UserId,
    string Email,
    string FullName,
    IReadOnlyCollection<string> Roles,
    string Phone,
    string Sport,
    string TargetArea,
    string ExperienceLevel,
    bool IsActive);
