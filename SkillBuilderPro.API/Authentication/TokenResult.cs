namespace SkillBuilderPro.API.Authentication;

public sealed record TokenResult(
    string AccessToken,
    DateTime ExpiresAtUtc);
