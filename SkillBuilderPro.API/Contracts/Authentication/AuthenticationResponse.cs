namespace SkillBuilderPro.API.Contracts.Authentication;

public sealed record AuthenticationResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    CurrentUserResponse User);
