using System.ComponentModel.DataAnnotations;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public class UserProfile
{
    [Key]
    public int UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Sport { get; set; } = string.Empty;

    [MaxLength(50)]
    public string TargetArea { get; set; } = string.Empty;

    [MaxLength(20)]
    public string ExperienceLevel { get; set; } = "Beginner";

    public bool IsActive { get; set; } = true;

    public string PhotoPath { get; set; } = string.Empty;

    public int Age { get; set; }

    public double Height { get; set; }

    public double Weight { get; set; }

    [MaxLength(60)]
    public string Team { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Bio { get; set; } = string.Empty;

    public int JerseyNumber { get; set; }

    [MaxLength(100)]
    public string Goal { get; set; } = string.Empty;

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
