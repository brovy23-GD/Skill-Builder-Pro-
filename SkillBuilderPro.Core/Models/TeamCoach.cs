using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public class TeamCoach
{
    public int TeamId { get; set; }
    public int CoachUserId { get; set; }

    [Required, MaxLength(30)]
    public string TeamRole { get; set; } = TeamRoles.AssistantCoach;

    public bool IsActive { get; set; } = true;
    public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;

    [JsonIgnore, ForeignKey(nameof(TeamId))]
    public Team Team { get; set; } = null!;

    [JsonIgnore, ForeignKey(nameof(CoachUserId))]
    public ApplicationUser CoachUser { get; set; } = null!;
}
