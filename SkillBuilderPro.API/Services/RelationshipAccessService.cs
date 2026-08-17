using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Security;

namespace SkillBuilderPro.API.Services;

public sealed class RelationshipAccessService : IRelationshipAccessService
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public RelationshipAccessService(
        AppDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<bool> CanParentAccessAthleteAsync(
        int parentUserId,
        int athleteUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsUserInRoleAsync(
                parentUserId,
                ApplicationRoles.Parent,
                cancellationToken)
            || !await IsUserInRoleAsync(
                athleteUserId,
                ApplicationRoles.Athlete,
                cancellationToken))
        {
            return false;
        }

        return await _dbContext.ParentAthletes.AnyAsync(
            link => link.ParentUserId == parentUserId
                && link.AthleteUserId == athleteUserId
                && link.IsActive,
            cancellationToken);
    }

    public async Task<bool> CanCoachAccessAthleteAsync(
        int coachUserId,
        int athleteUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsUserInRoleAsync(
                coachUserId,
                ApplicationRoles.Coach,
                cancellationToken)
            || !await IsUserInRoleAsync(
                athleteUserId,
                ApplicationRoles.Athlete,
                cancellationToken))
        {
            return false;
        }

        return await _dbContext.TeamCoaches.AnyAsync(
            coach => coach.CoachUserId == coachUserId
                && coach.IsActive
                && coach.Team.IsActive
                && coach.Team.Athletes.Any(
                    athlete => athlete.AthleteUserId == athleteUserId
                        && athlete.IsActive),
            cancellationToken);
    }

    public async Task<bool> CanCoachManageTeamAsync(
        int coachUserId,
        int teamId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsUserInRoleAsync(
                coachUserId,
                ApplicationRoles.Coach,
                cancellationToken))
        {
            return false;
        }

        return await _dbContext.TeamCoaches.AnyAsync(
            link => link.CoachUserId == coachUserId
                && link.TeamId == teamId
                && link.IsActive
                && link.Team.IsActive,
            cancellationToken);
    }

    public async Task<AthleteAccessScope> GetAccessibleAthleteIdsAsync(
        int actorUserId,
        CancellationToken cancellationToken = default)
    {
        var actor = await _userManager.FindByIdAsync(actorUserId.ToString());
        if (actor is null)
        {
            return new AthleteAccessScope(actorUserId, false, []);
        }

        var roles = await _userManager.GetRolesAsync(actor);
        if (roles.Contains(ApplicationRoles.Administrator))
        {
            return new AthleteAccessScope(actorUserId, true, []);
        }

        var athleteUserIds = new HashSet<int>();

        if (roles.Contains(ApplicationRoles.Athlete))
        {
            athleteUserIds.Add(actorUserId);
        }

        if (roles.Contains(ApplicationRoles.Parent))
        {
            var linkedAthleteIds = await _dbContext.ParentAthletes
                .Where(link => link.ParentUserId == actorUserId && link.IsActive)
                .Select(link => link.AthleteUserId)
                .ToListAsync(cancellationToken);

            athleteUserIds.UnionWith(linkedAthleteIds);
        }

        if (roles.Contains(ApplicationRoles.Coach))
        {
            var teamAthleteIds = await _dbContext.TeamCoaches
                .Where(coach => coach.CoachUserId == actorUserId
                    && coach.IsActive
                    && coach.Team.IsActive)
                .SelectMany(coach => coach.Team.Athletes)
                .Where(athlete => athlete.IsActive)
                .Select(athlete => athlete.AthleteUserId)
                .Distinct()
                .ToListAsync(cancellationToken);

            athleteUserIds.UnionWith(teamAthleteIds);
        }

        var roleValidatedAthleteIds = new HashSet<int>();
        foreach (var athleteUserId in athleteUserIds)
        {
            if (await IsUserInRoleAsync(
                    athleteUserId,
                    ApplicationRoles.Athlete,
                    cancellationToken))
            {
                roleValidatedAthleteIds.Add(athleteUserId);
            }
        }

        return new AthleteAccessScope(actorUserId, false, roleValidatedAthleteIds);
    }

    public async Task<bool> IsUserInRoleAsync(
        int userId,
        string expectedRole,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!ApplicationRoles.All.Contains(expectedRole))
        {
            return false;
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user is not null
            && await _userManager.IsInRoleAsync(user, expectedRole);
    }

    public async Task<IReadOnlyCollection<int>> GetParentAthleteIdsAsync(
        int parentUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsUserInRoleAsync(
                parentUserId,
                ApplicationRoles.Parent,
                cancellationToken))
        {
            return [];
        }

        var athleteIds = await _dbContext.ParentAthletes
            .AsNoTracking()
            .Where(link => link.ParentUserId == parentUserId && link.IsActive)
            .Select(link => link.AthleteUserId)
            .ToListAsync(cancellationToken);

        return await FilterAthleteRoleAsync(athleteIds, cancellationToken);
    }

    public async Task<IReadOnlyCollection<int>> GetCoachTeamIdsAsync(
        int coachUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsUserInRoleAsync(
                coachUserId,
                ApplicationRoles.Coach,
                cancellationToken))
        {
            return [];
        }

        return await _dbContext.TeamCoaches
            .AsNoTracking()
            .Where(link => link.CoachUserId == coachUserId
                && link.IsActive
                && link.Team.IsActive)
            .Select(link => link.TeamId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<int>> GetCoachTeamAthleteIdsAsync(
        int coachUserId,
        int teamId,
        CancellationToken cancellationToken = default)
    {
        if (!await CanCoachManageTeamAsync(
                coachUserId,
                teamId,
                cancellationToken))
        {
            return [];
        }

        var athleteIds = await _dbContext.TeamAthletes
            .AsNoTracking()
            .Where(link => link.TeamId == teamId
                && link.IsActive
                && link.Team.IsActive)
            .Select(link => link.AthleteUserId)
            .ToListAsync(cancellationToken);

        return await FilterAthleteRoleAsync(athleteIds, cancellationToken);
    }

    private async Task<IReadOnlyCollection<int>> FilterAthleteRoleAsync(
        IEnumerable<int> athleteIds,
        CancellationToken cancellationToken)
    {
        var validIds = new List<int>();
        foreach (var athleteId in athleteIds.Distinct())
        {
            if (await IsUserInRoleAsync(
                    athleteId,
                    ApplicationRoles.Athlete,
                    cancellationToken))
            {
                validIds.Add(athleteId);
            }
        }

        return validIds;
    }
}
