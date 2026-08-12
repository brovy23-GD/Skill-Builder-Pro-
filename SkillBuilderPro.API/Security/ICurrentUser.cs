namespace SkillBuilderPro.API.Security;

public interface ICurrentUser
{
    int? UserId { get; }
    bool IsAdministrator { get; }
}
