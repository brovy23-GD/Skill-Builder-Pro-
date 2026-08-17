using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.API.Contracts.Admin;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Services;

public sealed class AdminRelationshipService : IAdminRelationshipService
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminRelationshipService(
        AppDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IReadOnlyCollection<ParentAthleteResponse>> ListParentAthletesAsync(
        CancellationToken cancellationToken = default)
    {
        var links = await _dbContext.ParentAthletes
            .AsNoTracking()
            .OrderBy(link => link.ParentUserId)
            .ThenBy(link => link.AthleteUserId)
            .ToListAsync(cancellationToken);

        var responses = new List<ParentAthleteResponse>(links.Count);
        foreach (var link in links)
        {
            responses.Add(await MapParentAthleteAsync(link));
        }

        return responses;
    }

    public async Task<AdminServiceResult<ParentAthleteResponse>> GetParentAthleteAsync(
        int parentUserId,
        int athleteUserId,
        CancellationToken cancellationToken = default)
    {
        var link = await _dbContext.ParentAthletes
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.ParentUserId == parentUserId
                    && item.AthleteUserId == athleteUserId,
                cancellationToken);

        return link is null
            ? AdminServiceResult<ParentAthleteResponse>.NotFound("Parent/Athlete relationship was not found.")
            : AdminServiceResult<ParentAthleteResponse>.Success(await MapParentAthleteAsync(link));
    }

    public async Task<AdminServiceResult<ParentAthleteResponse>> CreateParentAthleteAsync(
        CreateParentAthleteRequest request,
        int administratorUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(administratorUserId))
        {
            return AdminServiceResult<ParentAthleteResponse>.Forbidden();
        }

        if (request.ParentUserId == request.AthleteUserId)
        {
            return AdminServiceResult<ParentAthleteResponse>.Validation(
                "Parent and Athlete must be different users.");
        }

        var roleFailure = await ValidateRelationshipRolesAsync(
            request.ParentUserId,
            ApplicationRoles.Parent,
            request.AthleteUserId,
            ApplicationRoles.Athlete);
        if (roleFailure is not null)
        {
            return ToRoleFailure<ParentAthleteResponse>(roleFailure);
        }

        var existing = await _dbContext.ParentAthletes.SingleOrDefaultAsync(
            link => link.ParentUserId == request.ParentUserId
                && link.AthleteUserId == request.AthleteUserId,
            cancellationToken);

        if (existing is not null)
        {
            if (existing.IsActive)
            {
                return AdminServiceResult<ParentAthleteResponse>.Conflict(
                    "An active Parent/Athlete relationship already exists.");
            }

            existing.IsActive = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return AdminServiceResult<ParentAthleteResponse>.Success(
                await MapParentAthleteAsync(existing));
        }

        var link = new ParentAthlete
        {
            ParentUserId = request.ParentUserId,
            AthleteUserId = request.AthleteUserId,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = administratorUserId
        };

        _dbContext.ParentAthletes.Add(link);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return AdminServiceResult<ParentAthleteResponse>.Conflict(
                "The Parent/Athlete relationship could not be created because it already exists or changed concurrently.");
        }

        return AdminServiceResult<ParentAthleteResponse>.Created(
            await MapParentAthleteAsync(link));
    }

    public async Task<AdminServiceResult<ParentAthleteResponse>> SetParentAthleteActiveAsync(
        int parentUserId,
        int athleteUserId,
        bool isActive,
        int administratorUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(administratorUserId))
        {
            return AdminServiceResult<ParentAthleteResponse>.Forbidden();
        }

        var link = await _dbContext.ParentAthletes.SingleOrDefaultAsync(
            item => item.ParentUserId == parentUserId && item.AthleteUserId == athleteUserId,
            cancellationToken);
        if (link is null)
        {
            return AdminServiceResult<ParentAthleteResponse>.NotFound(
                "Parent/Athlete relationship was not found.");
        }

        if (link.IsActive == isActive)
        {
            return AdminServiceResult<ParentAthleteResponse>.Conflict(
                $"Parent/Athlete relationship is already {(isActive ? "active" : "inactive")}.");
        }

        if (isActive)
        {
            var roleFailure = await ValidateRelationshipRolesAsync(
                parentUserId,
                ApplicationRoles.Parent,
                athleteUserId,
                ApplicationRoles.Athlete);
            if (roleFailure is not null)
            {
                return ToRoleFailure<ParentAthleteResponse>(roleFailure);
            }
        }

        link.IsActive = isActive;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return AdminServiceResult<ParentAthleteResponse>.Success(await MapParentAthleteAsync(link));
    }

    public async Task<IReadOnlyCollection<TeamResponse>> ListTeamsAsync(
        CancellationToken cancellationToken = default) =>
        await _dbContext.Teams
            .AsNoTracking()
            .OrderBy(team => team.Name)
            .Select(team => MapTeam(team))
            .ToListAsync(cancellationToken);

    public async Task<AdminServiceResult<TeamResponse>> GetTeamAsync(
        int teamId,
        CancellationToken cancellationToken = default)
    {
        var team = await _dbContext.Teams.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == teamId, cancellationToken);
        return team is null
            ? AdminServiceResult<TeamResponse>.NotFound("Team was not found.")
            : AdminServiceResult<TeamResponse>.Success(MapTeam(team));
    }

    public async Task<AdminServiceResult<TeamResponse>> CreateTeamAsync(
        CreateTeamRequest request,
        int administratorUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(administratorUserId))
        {
            return AdminServiceResult<TeamResponse>.Forbidden();
        }

        var normalized = NormalizeTeamFields(
            request.Name,
            request.Sport,
            request.Season,
            request.AgeGroup,
            request.Organization);
        if (normalized.Error is not null)
        {
            return AdminServiceResult<TeamResponse>.Validation(normalized.Error);
        }

        var team = new Team
        {
            Name = normalized.Name,
            Sport = normalized.Sport,
            Season = normalized.Season,
            AgeGroup = normalized.AgeGroup,
            Organization = normalized.Organization,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = administratorUserId
        };

        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return AdminServiceResult<TeamResponse>.Created(MapTeam(team));
    }

    public async Task<AdminServiceResult<TeamResponse>> UpdateTeamAsync(
        int teamId,
        UpdateTeamRequest request,
        int administratorUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(administratorUserId))
        {
            return AdminServiceResult<TeamResponse>.Forbidden();
        }

        var normalized = NormalizeTeamFields(
            request.Name,
            request.Sport,
            request.Season,
            request.AgeGroup,
            request.Organization);
        if (normalized.Error is not null)
        {
            return AdminServiceResult<TeamResponse>.Validation(normalized.Error);
        }

        var team = await _dbContext.Teams.SingleOrDefaultAsync(
            item => item.Id == teamId,
            cancellationToken);
        if (team is null)
        {
            return AdminServiceResult<TeamResponse>.NotFound("Team was not found.");
        }

        team.Name = normalized.Name;
        team.Sport = normalized.Sport;
        team.Season = normalized.Season;
        team.AgeGroup = normalized.AgeGroup;
        team.Organization = normalized.Organization;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return AdminServiceResult<TeamResponse>.Success(MapTeam(team));
    }

    public async Task<AdminServiceResult<TeamResponse>> SetTeamActiveAsync(
        int teamId,
        bool isActive,
        int administratorUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(administratorUserId))
        {
            return AdminServiceResult<TeamResponse>.Forbidden();
        }

        var team = await _dbContext.Teams.SingleOrDefaultAsync(
            item => item.Id == teamId,
            cancellationToken);
        if (team is null)
        {
            return AdminServiceResult<TeamResponse>.NotFound("Team was not found.");
        }

        if (team.IsActive == isActive)
        {
            return AdminServiceResult<TeamResponse>.Conflict(
                $"Team is already {(isActive ? "active" : "inactive")}.");
        }

        team.IsActive = isActive;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return AdminServiceResult<TeamResponse>.Success(MapTeam(team));
    }

    public async Task<AdminServiceResult<IReadOnlyCollection<TeamCoachResponse>>> ListTeamCoachesAsync(
        int teamId,
        CancellationToken cancellationToken = default)
    {
        if (!await _dbContext.Teams.AnyAsync(team => team.Id == teamId, cancellationToken))
        {
            return AdminServiceResult<IReadOnlyCollection<TeamCoachResponse>>.NotFound("Team was not found.");
        }

        var links = await _dbContext.TeamCoaches.AsNoTracking()
            .Where(link => link.TeamId == teamId)
            .OrderBy(link => link.CoachUserId)
            .ToListAsync(cancellationToken);
        var responses = new List<TeamCoachResponse>(links.Count);
        foreach (var link in links)
        {
            responses.Add(await MapTeamCoachAsync(link));
        }

        return AdminServiceResult<IReadOnlyCollection<TeamCoachResponse>>.Success(responses);
    }

    public async Task<AdminServiceResult<TeamCoachResponse>> AddTeamCoachAsync(
        int teamId,
        AddTeamCoachRequest request,
        int administratorUserId,
        CancellationToken cancellationToken = default)
    {
        var authorization = await ValidateAdminAndActiveTeamAsync<TeamCoachResponse>(
            administratorUserId,
            teamId,
            cancellationToken);
        if (authorization is not null)
        {
            return authorization;
        }

        var coachRoleFailure = await ValidateUserRoleAsync(
            request.CoachUserId,
            ApplicationRoles.Coach);
        if (coachRoleFailure is not null)
        {
            return ToRoleFailure<TeamCoachResponse>(coachRoleFailure);
        }

        var teamRole = NormalizeTeamRole(request.TeamRole);
        if (teamRole is null)
        {
            return AdminServiceResult<TeamCoachResponse>.Validation("TeamRole is invalid.");
        }

        var existing = await _dbContext.TeamCoaches.SingleOrDefaultAsync(
            link => link.TeamId == teamId && link.CoachUserId == request.CoachUserId,
            cancellationToken);
        if (existing is not null)
        {
            if (existing.IsActive)
            {
                return AdminServiceResult<TeamCoachResponse>.Conflict(
                    "Coach is already an active member of the Team.");
            }

            existing.IsActive = true;
            existing.TeamRole = teamRole;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return AdminServiceResult<TeamCoachResponse>.Success(await MapTeamCoachAsync(existing));
        }

        var link = new TeamCoach
        {
            TeamId = teamId,
            CoachUserId = request.CoachUserId,
            TeamRole = teamRole,
            IsActive = true,
            JoinedAtUtc = DateTime.UtcNow
        };
        _dbContext.TeamCoaches.Add(link);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return AdminServiceResult<TeamCoachResponse>.Conflict(
                "The Team/Coach membership could not be created because it already exists or changed concurrently.");
        }

        return AdminServiceResult<TeamCoachResponse>.Created(await MapTeamCoachAsync(link));
    }

    public async Task<AdminServiceResult<TeamCoachResponse>> UpdateTeamCoachRoleAsync(
        int teamId,
        int coachUserId,
        UpdateTeamCoachRequest request,
        int administratorUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(administratorUserId))
        {
            return AdminServiceResult<TeamCoachResponse>.Forbidden();
        }

        var teamRole = NormalizeTeamRole(request.TeamRole);
        if (teamRole is null)
        {
            return AdminServiceResult<TeamCoachResponse>.Validation("TeamRole is invalid.");
        }

        var link = await _dbContext.TeamCoaches.SingleOrDefaultAsync(
            item => item.TeamId == teamId && item.CoachUserId == coachUserId,
            cancellationToken);
        if (link is null)
        {
            return AdminServiceResult<TeamCoachResponse>.NotFound("Team/Coach membership was not found.");
        }

        if (!await UserHasRoleAsync(coachUserId, ApplicationRoles.Coach))
        {
            return AdminServiceResult<TeamCoachResponse>.Validation(
                "Coach user no longer has the Coach role.");
        }

        link.TeamRole = teamRole;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return AdminServiceResult<TeamCoachResponse>.Success(await MapTeamCoachAsync(link));
    }

    public async Task<AdminServiceResult<TeamCoachResponse>> SetTeamCoachActiveAsync(
        int teamId,
        int coachUserId,
        bool isActive,
        int administratorUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(administratorUserId))
        {
            return AdminServiceResult<TeamCoachResponse>.Forbidden();
        }

        var link = await _dbContext.TeamCoaches
            .Include(item => item.Team)
            .SingleOrDefaultAsync(
                item => item.TeamId == teamId && item.CoachUserId == coachUserId,
                cancellationToken);
        if (link is null)
        {
            return AdminServiceResult<TeamCoachResponse>.NotFound("Team/Coach membership was not found.");
        }

        if (link.IsActive == isActive)
        {
            return AdminServiceResult<TeamCoachResponse>.Conflict(
                $"Team/Coach membership is already {(isActive ? "active" : "inactive")}.");
        }

        if (isActive)
        {
            if (!link.Team.IsActive)
            {
                return AdminServiceResult<TeamCoachResponse>.Conflict(
                    "An inactive Team cannot have a Coach membership reactivated.");
            }

            if (!await UserHasRoleAsync(coachUserId, ApplicationRoles.Coach))
            {
                return AdminServiceResult<TeamCoachResponse>.Validation(
                    "Coach user no longer has the Coach role.");
            }
        }

        link.IsActive = isActive;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return AdminServiceResult<TeamCoachResponse>.Success(await MapTeamCoachAsync(link));
    }

    public async Task<AdminServiceResult<IReadOnlyCollection<TeamAthleteResponse>>> ListTeamAthletesAsync(
        int teamId,
        CancellationToken cancellationToken = default)
    {
        if (!await _dbContext.Teams.AnyAsync(team => team.Id == teamId, cancellationToken))
        {
            return AdminServiceResult<IReadOnlyCollection<TeamAthleteResponse>>.NotFound("Team was not found.");
        }

        var links = await _dbContext.TeamAthletes.AsNoTracking()
            .Where(link => link.TeamId == teamId)
            .OrderBy(link => link.AthleteUserId)
            .ToListAsync(cancellationToken);
        var responses = new List<TeamAthleteResponse>(links.Count);
        foreach (var link in links)
        {
            responses.Add(await MapTeamAthleteAsync(link));
        }

        return AdminServiceResult<IReadOnlyCollection<TeamAthleteResponse>>.Success(responses);
    }

    public async Task<AdminServiceResult<TeamAthleteResponse>> AddTeamAthleteAsync(
        int teamId,
        AddTeamAthleteRequest request,
        int administratorUserId,
        CancellationToken cancellationToken = default)
    {
        var authorization = await ValidateAdminAndActiveTeamAsync<TeamAthleteResponse>(
            administratorUserId,
            teamId,
            cancellationToken);
        if (authorization is not null)
        {
            return authorization;
        }

        var athleteRoleFailure = await ValidateUserRoleAsync(
            request.AthleteUserId,
            ApplicationRoles.Athlete);
        if (athleteRoleFailure is not null)
        {
            return ToRoleFailure<TeamAthleteResponse>(athleteRoleFailure);
        }

        var existing = await _dbContext.TeamAthletes.SingleOrDefaultAsync(
            link => link.TeamId == teamId && link.AthleteUserId == request.AthleteUserId,
            cancellationToken);
        if (existing is not null)
        {
            if (existing.IsActive)
            {
                return AdminServiceResult<TeamAthleteResponse>.Conflict(
                    "Athlete is already an active member of the Team.");
            }

            existing.IsActive = true;
            existing.LeftAtUtc = null;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return AdminServiceResult<TeamAthleteResponse>.Success(await MapTeamAthleteAsync(existing));
        }

        var link = new TeamAthlete
        {
            TeamId = teamId,
            AthleteUserId = request.AthleteUserId,
            IsActive = true,
            JoinedAtUtc = DateTime.UtcNow,
            LeftAtUtc = null
        };
        _dbContext.TeamAthletes.Add(link);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return AdminServiceResult<TeamAthleteResponse>.Conflict(
                "The Team/Athlete membership could not be created because it already exists or changed concurrently.");
        }

        return AdminServiceResult<TeamAthleteResponse>.Created(await MapTeamAthleteAsync(link));
    }

    public async Task<AdminServiceResult<TeamAthleteResponse>> SetTeamAthleteActiveAsync(
        int teamId,
        int athleteUserId,
        bool isActive,
        int administratorUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(administratorUserId))
        {
            return AdminServiceResult<TeamAthleteResponse>.Forbidden();
        }

        var link = await _dbContext.TeamAthletes
            .Include(item => item.Team)
            .SingleOrDefaultAsync(
                item => item.TeamId == teamId && item.AthleteUserId == athleteUserId,
                cancellationToken);
        if (link is null)
        {
            return AdminServiceResult<TeamAthleteResponse>.NotFound("Team/Athlete membership was not found.");
        }

        if (link.IsActive == isActive)
        {
            return AdminServiceResult<TeamAthleteResponse>.Conflict(
                $"Team/Athlete membership is already {(isActive ? "active" : "inactive")}.");
        }

        if (isActive)
        {
            if (!link.Team.IsActive)
            {
                return AdminServiceResult<TeamAthleteResponse>.Conflict(
                    "An inactive Team cannot have an Athlete membership reactivated.");
            }

            if (!await UserHasRoleAsync(athleteUserId, ApplicationRoles.Athlete))
            {
                return AdminServiceResult<TeamAthleteResponse>.Validation(
                    "Athlete user no longer has the Athlete role.");
            }
        }

        link.IsActive = isActive;
        link.LeftAtUtc = isActive ? null : DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return AdminServiceResult<TeamAthleteResponse>.Success(await MapTeamAthleteAsync(link));
    }

    private async Task<AdminServiceResult<T>?> ValidateAdminAndActiveTeamAsync<T>(
        int administratorUserId,
        int teamId,
        CancellationToken cancellationToken)
    {
        if (!await IsAdministratorAsync(administratorUserId))
        {
            return AdminServiceResult<T>.Forbidden();
        }

        var team = await _dbContext.Teams.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == teamId, cancellationToken);
        if (team is null)
        {
            return AdminServiceResult<T>.NotFound("Team was not found.");
        }

        return team.IsActive
            ? null
            : AdminServiceResult<T>.Conflict("Membership cannot be added to an inactive Team.");
    }

    private async Task<RoleValidationFailure?> ValidateRelationshipRolesAsync(
        int firstUserId,
        string firstRole,
        int secondUserId,
        string secondRole)
    {
        var firstFailure = await ValidateUserRoleAsync(firstUserId, firstRole);
        if (firstFailure is not null)
        {
            return firstFailure;
        }

        return await ValidateUserRoleAsync(secondUserId, secondRole);
    }

    private async Task<RoleValidationFailure?> ValidateUserRoleAsync(int userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return new RoleValidationFailure(
                AdminServiceStatus.NotFound,
                $"User {userId} was not found.");
        }

        return await _userManager.IsInRoleAsync(user, role)
            ? null
            : new RoleValidationFailure(
                AdminServiceStatus.ValidationError,
                $"User {userId} does not have the {role} role.");
    }

    private static AdminServiceResult<T> ToRoleFailure<T>(RoleValidationFailure failure) =>
        failure.Status == AdminServiceStatus.NotFound
            ? AdminServiceResult<T>.NotFound(failure.Error)
            : AdminServiceResult<T>.Validation(failure.Error);

    private Task<bool> IsAdministratorAsync(int userId) =>
        UserHasRoleAsync(userId, ApplicationRoles.Administrator);

    private async Task<bool> UserHasRoleAsync(int userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user is not null && await _userManager.IsInRoleAsync(user, role);
    }

    private async Task<AdminUserSummary> MapUserAsync(int userId, string expectedRole)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return new AdminUserSummary(userId, string.Empty, string.Empty, expectedRole, false);
        }

        var displayName = await _dbContext.UserProfiles.AsNoTracking()
            .Where(profile => profile.UserId == userId)
            .Select(profile => profile.FullName)
            .SingleOrDefaultAsync() ?? string.Empty;

        return new AdminUserSummary(
            userId,
            displayName,
            user.Email ?? string.Empty,
            expectedRole,
            await _userManager.IsInRoleAsync(user, expectedRole));
    }

    private async Task<ParentAthleteResponse> MapParentAthleteAsync(ParentAthlete link) =>
        new(
            await MapUserAsync(link.ParentUserId, ApplicationRoles.Parent),
            await MapUserAsync(link.AthleteUserId, ApplicationRoles.Athlete),
            link.IsActive,
            link.CreatedAtUtc,
            link.CreatedByUserId);

    private async Task<TeamCoachResponse> MapTeamCoachAsync(TeamCoach link) =>
        new(
            link.TeamId,
            await MapUserAsync(link.CoachUserId, ApplicationRoles.Coach),
            link.TeamRole,
            link.IsActive,
            link.JoinedAtUtc);

    private async Task<TeamAthleteResponse> MapTeamAthleteAsync(TeamAthlete link) =>
        new(
            link.TeamId,
            await MapUserAsync(link.AthleteUserId, ApplicationRoles.Athlete),
            link.IsActive,
            link.JoinedAtUtc,
            link.LeftAtUtc);

    private static TeamResponse MapTeam(Team team) =>
        new(
            team.Id,
            team.Name,
            team.Sport,
            team.Season,
            team.AgeGroup,
            team.Organization,
            team.IsActive,
            team.CreatedAtUtc,
            team.CreatedByUserId);

    private static string? NormalizeTeamRole(string teamRole) =>
        TeamRoles.All.FirstOrDefault(
            allowed => string.Equals(allowed, teamRole.Trim(), StringComparison.OrdinalIgnoreCase));

    private static NormalizedTeamFields NormalizeTeamFields(
        string name,
        string sport,
        string? season,
        string? ageGroup,
        string? organization)
    {
        var normalizedName = name.Trim();
        var normalizedSport = sport.Trim();
        if (normalizedName.Length == 0)
        {
            return new("", "", null, null, null, "Name is required.");
        }

        if (normalizedSport.Length == 0)
        {
            return new("", "", null, null, null, "Sport is required.");
        }

        return new(
            normalizedName,
            normalizedSport,
            NullIfWhiteSpace(season),
            NullIfWhiteSpace(ageGroup),
            NullIfWhiteSpace(organization),
            null);
    }

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record NormalizedTeamFields(
        string Name,
        string Sport,
        string? Season,
        string? AgeGroup,
        string? Organization,
        string? Error);

    private sealed record RoleValidationFailure(AdminServiceStatus Status, string Error);
}
