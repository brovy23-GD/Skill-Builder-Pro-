namespace SkillBuilderPro.API.Contracts.Admin;

public sealed record AdminRoleChangeRequest(string Role, string Reason);
public sealed record AdminStatusChangeRequest(bool IsActive, string Reason);
public sealed record AdminDrillRequest(string Name, string Sport, string Category, string? SubCategory, string? Description, int? Difficulty, string? Duration, string VideoUrl, string Reason);
