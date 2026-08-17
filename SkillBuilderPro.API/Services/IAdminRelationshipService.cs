using SkillBuilderPro.API.Contracts.Admin;

namespace SkillBuilderPro.API.Services;

public interface IAdminRelationshipService
{
    Task<IReadOnlyCollection<ParentAthleteResponse>> ListParentAthletesAsync(CancellationToken cancellationToken = default);
    Task<AdminServiceResult<ParentAthleteResponse>> GetParentAthleteAsync(int parentUserId, int athleteUserId, CancellationToken cancellationToken = default);
    Task<AdminServiceResult<ParentAthleteResponse>> CreateParentAthleteAsync(CreateParentAthleteRequest request, int administratorUserId, CancellationToken cancellationToken = default);
    Task<AdminServiceResult<ParentAthleteResponse>> SetParentAthleteActiveAsync(int parentUserId, int athleteUserId, bool isActive, int administratorUserId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TeamResponse>> ListTeamsAsync(CancellationToken cancellationToken = default);
    Task<AdminServiceResult<TeamResponse>> GetTeamAsync(int teamId, CancellationToken cancellationToken = default);
    Task<AdminServiceResult<TeamResponse>> CreateTeamAsync(CreateTeamRequest request, int administratorUserId, CancellationToken cancellationToken = default);
    Task<AdminServiceResult<TeamResponse>> UpdateTeamAsync(int teamId, UpdateTeamRequest request, int administratorUserId, CancellationToken cancellationToken = default);
    Task<AdminServiceResult<TeamResponse>> SetTeamActiveAsync(int teamId, bool isActive, int administratorUserId, CancellationToken cancellationToken = default);

    Task<AdminServiceResult<IReadOnlyCollection<TeamCoachResponse>>> ListTeamCoachesAsync(int teamId, CancellationToken cancellationToken = default);
    Task<AdminServiceResult<TeamCoachResponse>> AddTeamCoachAsync(int teamId, AddTeamCoachRequest request, int administratorUserId, CancellationToken cancellationToken = default);
    Task<AdminServiceResult<TeamCoachResponse>> UpdateTeamCoachRoleAsync(int teamId, int coachUserId, UpdateTeamCoachRequest request, int administratorUserId, CancellationToken cancellationToken = default);
    Task<AdminServiceResult<TeamCoachResponse>> SetTeamCoachActiveAsync(int teamId, int coachUserId, bool isActive, int administratorUserId, CancellationToken cancellationToken = default);

    Task<AdminServiceResult<IReadOnlyCollection<TeamAthleteResponse>>> ListTeamAthletesAsync(int teamId, CancellationToken cancellationToken = default);
    Task<AdminServiceResult<TeamAthleteResponse>> AddTeamAthleteAsync(int teamId, AddTeamAthleteRequest request, int administratorUserId, CancellationToken cancellationToken = default);
    Task<AdminServiceResult<TeamAthleteResponse>> SetTeamAthleteActiveAsync(int teamId, int athleteUserId, bool isActive, int administratorUserId, CancellationToken cancellationToken = default);
}
