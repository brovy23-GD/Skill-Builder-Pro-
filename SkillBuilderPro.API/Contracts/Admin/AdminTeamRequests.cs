using System.ComponentModel.DataAnnotations;

namespace SkillBuilderPro.API.Contracts.Admin;

public sealed class CreateTeamRequest
{
    [Required, MaxLength(120)]
    public string Name { get; init; } = string.Empty;

    [Required, MaxLength(50)]
    public string Sport { get; init; } = string.Empty;

    [MaxLength(50)]
    public string? Season { get; init; }

    [MaxLength(50)]
    public string? AgeGroup { get; init; }

    [MaxLength(150)]
    public string? Organization { get; init; }
}

public sealed class UpdateTeamRequest
{
    [Required, MaxLength(120)]
    public string Name { get; init; } = string.Empty;

    [Required, MaxLength(50)]
    public string Sport { get; init; } = string.Empty;

    [MaxLength(50)]
    public string? Season { get; init; }

    [MaxLength(50)]
    public string? AgeGroup { get; init; }

    [MaxLength(150)]
    public string? Organization { get; init; }
}
