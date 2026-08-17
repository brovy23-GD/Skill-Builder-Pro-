using SkillBuilderPro.Core.Security;

namespace SkillBuilderPro.Core.Interfaces;

public interface IRelationshipAccessService
{
    Task<bool> CanParentAccessAthleteAsync(
        int parentUserId,
        int athleteUserId,
        CancellationToken cancellationToken = default);

    Task<bool> CanCoachAccessAthleteAsync(
        int coachUserId,
        int athleteUserId,
        CancellationToken cancellationToken = default);

    Task<bool> CanCoachManageTeamAsync(
        int coachUserId,
        int teamId,
        CancellationToken cancellationToken = default);

    Task<AthleteAccessScope> GetAccessibleAthleteIdsAsync(
        int actorUserId,
        CancellationToken cancellationToken = default);

    Task<bool> IsUserInRoleAsync(
        int userId,
        string expectedRole,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<int>> GetParentAthleteIdsAsync(
        int parentUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<int>> GetCoachTeamIdsAsync(
        int coachUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<int>> GetCoachTeamAthleteIdsAsync(
        int coachUserId,
        int teamId,
        CancellationToken cancellationToken = default);
}
