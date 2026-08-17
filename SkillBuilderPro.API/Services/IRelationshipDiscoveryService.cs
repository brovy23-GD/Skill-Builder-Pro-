using SkillBuilderPro.API.Contracts.Access;

namespace SkillBuilderPro.API.Services;

public interface IRelationshipDiscoveryService
{
    Task<IReadOnlyCollection<AthleteSummaryResponse>> GetParentAthletesAsync(int parentUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CoachTeamResponse>> GetCoachTeamsAsync(int coachUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AthleteSummaryResponse>?> GetCoachTeamRosterAsync(int coachUserId, int teamId, CancellationToken cancellationToken = default);
}
