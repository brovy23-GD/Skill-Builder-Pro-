using System.ComponentModel.DataAnnotations;

namespace SkillBuilderPro.Core.Models;

public class Drill
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Sport { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;   // e.g., Dribbling, Hitting, Footwork

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(200)]
    public string VideoUrl { get; set; } = string.Empty;   // YouTube link (next phase: YouTube API integration)

    [Range(1, 5)]
    public int DifficultyLevel { get; set; } = 1;           // 1 = Beginner, 5 = Elite
    [Range(1, 240)]
    public int Duration { get; set; } = 20;                 // minutes
    [MaxLength(50)]
    public string SubCategory { get; set; } = string.Empty;  // e.g., Shooting, Dribbling, Footwork
    public DateTime DateCreated { get; set; } = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc);
    public List<TrainingSchedule> Schedules { get; set; } = new();
    public List<ProgressLog> ProgressLogs { get; set; } = new();
}