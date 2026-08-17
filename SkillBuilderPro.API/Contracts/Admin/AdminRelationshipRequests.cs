using System.ComponentModel.DataAnnotations;

namespace SkillBuilderPro.API.Contracts.Admin;

public sealed class CreateParentAthleteRequest
{
    [Range(1, int.MaxValue)]
    public int ParentUserId { get; init; }

    [Range(1, int.MaxValue)]
    public int AthleteUserId { get; init; }
}

public sealed class AddTeamCoachRequest
{
    [Range(1, int.MaxValue)]
    public int CoachUserId { get; init; }

    [Required, MaxLength(30)]
    public string TeamRole { get; init; } = string.Empty;
}

public sealed class UpdateTeamCoachRequest
{
    [Required, MaxLength(30)]
    public string TeamRole { get; init; } = string.Empty;
}

public sealed class AddTeamAthleteRequest
{
    [Range(1, int.MaxValue)]
    public int AthleteUserId { get; init; }
}
