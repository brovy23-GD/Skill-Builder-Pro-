using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public class TeamAthlete
{
    public int TeamId { get; set; }
    public int AthleteUserId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LeftAtUtc { get; set; }

    [JsonIgnore, ForeignKey(nameof(TeamId))]
    public Team Team { get; set; } = null!;

    [JsonIgnore, ForeignKey(nameof(AthleteUserId))]
    public ApplicationUser AthleteUser { get; set; } = null!;
}
