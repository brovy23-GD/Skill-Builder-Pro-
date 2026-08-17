using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public class ParentAthlete
{
    public int ParentUserId { get; set; }
    public int AthleteUserId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public int CreatedByUserId { get; set; }

    [JsonIgnore, ForeignKey(nameof(ParentUserId))]
    public ApplicationUser ParentUser { get; set; } = null!;

    [JsonIgnore, ForeignKey(nameof(AthleteUserId))]
    public ApplicationUser AthleteUser { get; set; } = null!;

    [JsonIgnore, ForeignKey(nameof(CreatedByUserId))]
    public ApplicationUser CreatedByUser { get; set; } = null!;
}
