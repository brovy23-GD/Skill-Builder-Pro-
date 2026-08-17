using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.API.Contracts.Access;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Interfaces;

namespace SkillBuilderPro.API.Services;

public sealed class RelationshipDiscoveryService : IRelationshipDiscoveryService
{
    private readonly AppDbContext _dbContext;
    private readonly IRelationshipAccessService _access;

    public RelationshipDiscoveryService(AppDbContext dbContext, IRelationshipAccessService access)
    {
        _dbContext = dbContext;
        _access = access;
    }

    public async Task<IReadOnlyCollection<AthleteSummaryResponse>> GetParentAthletesAsync(int parentUserId, CancellationToken cancellationToken = default) =>
        await GetAthletesAsync(await _access.GetParentAthleteIdsAsync(parentUserId, cancellationToken), cancellationToken);

    public async Task<IReadOnlyCollection<CoachTeamResponse>> GetCoachTeamsAsync(int coachUserId, CancellationToken cancellationToken = default)
    {
        var ids = await _access.GetCoachTeamIdsAsync(coachUserId, cancellationToken);
        return await _dbContext.Teams.AsNoTracking()
            .Where(team => ids.Contains(team.Id) && team.IsActive)
            .OrderBy(team => team.Name)
            .Select(team => new CoachTeamResponse(team.Id, team.Name, team.Sport, team.Season, team.AgeGroup, team.Organization))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AthleteSummaryResponse>?> GetCoachTeamRosterAsync(int coachUserId, int teamId, CancellationToken cancellationToken = default)
    {
        if (!await _access.CanCoachManageTeamAsync(coachUserId, teamId, cancellationToken)) return null;
        var ids = await _access.GetCoachTeamAthleteIdsAsync(coachUserId, teamId, cancellationToken);
        return await GetAthletesAsync(ids, cancellationToken);
    }

    private async Task<IReadOnlyCollection<AthleteSummaryResponse>> GetAthletesAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken) =>
        await _dbContext.UserProfiles.AsNoTracking()
            .Where(profile => ids.Contains(profile.UserId) && profile.IsActive)
            .OrderBy(profile => profile.FullName)
            .Select(profile => new AthleteSummaryResponse(profile.UserId, profile.FullName, profile.Sport, profile.ExperienceLevel))
            .ToListAsync(cancellationToken);
}
