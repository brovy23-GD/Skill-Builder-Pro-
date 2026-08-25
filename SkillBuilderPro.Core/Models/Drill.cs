using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkillBuilderPro.Core.Models;

public class Drill
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [MaxLength(100)]
    [JsonPropertyName("externalSourceKey")]
    public string? ExternalSourceKey { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    // Level 1
    // Example: BASKETBALL, FOOTBALL, BASEBALL
    [JsonPropertyName("sport")]
    public string Sport { get; set; } = string.Empty;

    // Level 2
    // Example: Offense, Defense, Workout
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    // Level 3
    // Example: Foundational Skills, Position-Specific Skills,
    // Infield, Hitting Fundamentals, QB Drills, etc.
    [JsonPropertyName("drillGroup")]
    public string? DrillGroup { get; set; }

    // Level 4
    // Example: Shooting, Defensive Rebounding,
    // Ground Balls, WR, Tackling, etc.
    [JsonPropertyName("subCategory")]
    public string? SubCategory { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("difficulty")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? Difficulty { get; set; }
    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    [JsonPropertyName("videoUrl")]
    public string? VideoUrl { get; set; }

    [JsonPropertyName("dateCreated")]
    public DateTime? DateCreated { get; set; }

    [JsonIgnore]
    public ICollection<TrainingSchedule> Schedules { get; set; }
        = new List<TrainingSchedule>();

    [JsonIgnore]
    public ICollection<ProgressLog> ProgressLogs { get; set; }
        = new List<ProgressLog>();
}
