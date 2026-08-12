// Location: SkillBuilderPro.Core/Models/TrainingSchedule.cs
using System;
using System.Text.Json.Serialization;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public class TrainingSchedule
{
    public int Id { get; set; }
    public int DrillId { get; set; }

    // 🟢 ELITE FIX: Add missing parameters to satisfy ScheduleService evaluations
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public int? OwnerUserId { get; set; }

    [JsonIgnore]
    public ApplicationUser? Owner { get; set; }

    // Leave your existing properties (like dates or foreign keys) intact underneath
}
