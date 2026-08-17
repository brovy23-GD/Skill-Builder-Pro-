using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public class Team
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Sport { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Season { get; set; }

    [MaxLength(50)]
    public string? AgeGroup { get; set; }

    [MaxLength(150)]
    public string? Organization { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public int CreatedByUserId { get; set; }

    [JsonIgnore, ForeignKey(nameof(CreatedByUserId))]
    public ApplicationUser CreatedByUser { get; set; } = null!;

    [JsonIgnore]
    public ICollection<TeamCoach> Coaches { get; set; } = [];

    [JsonIgnore]
    public ICollection<TeamAthlete> Athletes { get; set; } = [];
}
