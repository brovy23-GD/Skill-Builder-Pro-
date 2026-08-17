namespace SkillBuilderPro.Core.Security;

public sealed class AthleteAccessScope
{
    public AthleteAccessScope(
        int actorUserId,
        bool isAdministrator,
        IEnumerable<int> athleteUserIds)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(actorUserId);

        ActorUserId = actorUserId;
        IsAdministrator = isAdministrator;
        AthleteUserIds = athleteUserIds.Distinct().ToHashSet();
    }

    public int ActorUserId { get; }
    public bool IsAdministrator { get; }
    public IReadOnlySet<int> AthleteUserIds { get; }

    public bool CanAccessAthlete(int athleteUserId) =>
        IsAdministrator || AthleteUserIds.Contains(athleteUserId);
}
