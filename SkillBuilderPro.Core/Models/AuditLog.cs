using System.ComponentModel.DataAnnotations;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.Core.Models;

public sealed class AuditLog
{
    public long AuditLogId { get; set; }
    public int AdministratorUserId { get; set; }
    public ApplicationUser AdministratorUser { get; set; } = null!;
    [MaxLength(100)] public string Action { get; set; } = string.Empty;
    [MaxLength(100)] public string ResourceType { get; set; } = string.Empty;
    [MaxLength(100)] public string ResourceId { get; set; } = string.Empty;
    public string? BeforeData { get; set; }
    public string? AfterData { get; set; }
    [MaxLength(500)] public string? Reason { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}
