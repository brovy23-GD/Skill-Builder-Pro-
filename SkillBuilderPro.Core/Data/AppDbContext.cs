using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Models;
using System.Collections.Generic;

namespace SkillBuilderPro.Core.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Drill> Drills { get; set; } = null!;
    public DbSet<ProgressLog> ProgressLogs { get; set; } = null!;
    public DbSet<User> LegacyUsers { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<TrainingSchedule> Schedules { get; set; } = null!;
    public DbSet<ParentAthlete> ParentAthletes { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<TeamCoach> TeamCoaches { get; set; } = null!;
    public DbSet<TeamAthlete> TeamAthletes { get; set; } = null!;
    public DbSet<DrillAssignment> DrillAssignments { get; set; } = null!;
    public DbSet<DrillAssignmentRecipient> DrillAssignmentRecipients { get; set; } = null!;
    public DbSet<AssignmentCompletionEvent> AssignmentCompletionEvents { get; set; } = null!;
    public DbSet<AthleteProgression> AthleteProgressions { get; set; } = null!;
    public DbSet<AthleteSkillProgress> AthleteSkillProgress { get; set; } = null!;
    public DbSet<AthleteRankHistory> AthleteRankHistories { get; set; } = null!;
    public DbSet<AthleteSkillLevelHistory> AthleteSkillLevelHistories { get; set; } = null!;
    public DbSet<AchievementDefinition> AchievementDefinitions { get; set; } = null!;
    public DbSet<AthleteAchievement> AthleteAchievements { get; set; } = null!;
    public DbSet<AthleteGoal> AthleteGoals { get; set; } = null!;
    public DbSet<TrainingRequest> TrainingRequests { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationEvent> NotificationEvents { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .ToTable("Users");

        modelBuilder.Entity<Drill>(entity =>
        {
            entity.HasIndex(drill => drill.ExternalSourceKey)
                .IsUnique()
                .HasFilter("[ExternalSourceKey] IS NOT NULL");
        });

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(user => user.Profile)
            .WithOne(profile => profile.User)
            .HasForeignKey<UserProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(log => log.TimestampUtc);
            entity.HasIndex(log => new { log.ResourceType, log.ResourceId });
            entity.HasOne(log => log.AdministratorUser).WithMany()
                .HasForeignKey(log => log.AdministratorUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ProgressLog>()
            .HasOne(log => log.Owner)
            .WithMany()
            .HasForeignKey(log => log.OwnerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ProgressLog>(entity =>
        {
            entity.HasIndex(log => log.AssignmentCompletionEventId)
                .IsUnique()
                .HasFilter("[AssignmentCompletionEventId] IS NOT NULL");

            entity.HasOne(log => log.AssignmentCompletionEvent)
                .WithMany()
                .HasForeignKey(log => log.AssignmentCompletionEventId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<TrainingSchedule>()
            .HasOne(schedule => schedule.Owner)
            .WithMany()
            .HasForeignKey(schedule => schedule.OwnerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ParentAthlete>(entity =>
        {
            entity.HasKey(link => new { link.ParentUserId, link.AthleteUserId });
            entity.HasIndex(link => link.AthleteUserId);

            entity.HasOne(link => link.ParentUser)
                .WithMany()
                .HasForeignKey(link => link.ParentUserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(link => link.AthleteUser)
                .WithMany()
                .HasForeignKey(link => link.AthleteUserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(link => link.CreatedByUser)
                .WithMany()
                .HasForeignKey(link => link.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasIndex(team => new { team.Sport, team.IsActive });

            entity.HasOne(team => team.CreatedByUser)
                .WithMany()
                .HasForeignKey(team => team.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<TeamCoach>(entity =>
        {
            entity.HasKey(link => new { link.TeamId, link.CoachUserId });
            entity.HasIndex(link => link.CoachUserId);

            entity.HasOne(link => link.Team)
                .WithMany(team => team.Coaches)
                .HasForeignKey(link => link.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(link => link.CoachUser)
                .WithMany()
                .HasForeignKey(link => link.CoachUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<TeamAthlete>(entity =>
        {
            entity.HasKey(link => new { link.TeamId, link.AthleteUserId });
            entity.HasIndex(link => link.AthleteUserId);

            entity.HasOne(link => link.Team)
                .WithMany(team => team.Athletes)
                .HasForeignKey(link => link.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(link => link.AthleteUser)
                .WithMany()
                .HasForeignKey(link => link.AthleteUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<DrillAssignment>(entity =>
        {
            entity.HasIndex(assignment => new { assignment.AssignedByUserId, assignment.CreatedAtUtc });
            entity.HasIndex(assignment => new { assignment.SourceTeamId, assignment.ScheduledForUtc });

            entity.ToTable(table => table.HasCheckConstraint(
                "CK_DrillAssignments_Status",
                "[Status] IN ('Scheduled', 'Active', 'Cancelled', 'Closed')"));

            entity.HasOne(assignment => assignment.Drill)
                .WithMany()
                .HasForeignKey(assignment => assignment.DrillId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(assignment => assignment.AssignedByUser)
                .WithMany()
                .HasForeignKey(assignment => assignment.AssignedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(assignment => assignment.SourceTeam)
                .WithMany()
                .HasForeignKey(assignment => assignment.SourceTeamId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<DrillAssignmentRecipient>(entity =>
        {
            entity.HasKey(recipient => new { recipient.AssignmentId, recipient.AthleteUserId });
            entity.HasIndex(recipient => new { recipient.AthleteUserId, recipient.Status, recipient.AssignmentId });
            entity.HasIndex(recipient => new { recipient.AthleteUserId, recipient.CompletedAtUtc });

            entity.ToTable(table => table.HasCheckConstraint(
                "CK_DrillAssignmentRecipients_Status",
                "[Status] IN ('Assigned', 'InProgress', 'Completed', 'Missed', 'Excused')"));
            entity.ToTable(table => table.HasCheckConstraint(
                "CK_DrillAssignmentRecipients_Rating",
                "[Rating] IS NULL OR ([Rating] >= 1 AND [Rating] <= 5)"));

            entity.HasOne(recipient => recipient.Assignment)
                .WithMany(assignment => assignment.Recipients)
                .HasForeignKey(recipient => recipient.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(recipient => recipient.AthleteUser)
                .WithMany()
                .HasForeignKey(recipient => recipient.AthleteUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AssignmentCompletionEvent>(entity =>
        {
            entity.HasIndex(completionEvent => new
                {
                    completionEvent.AssignmentId,
                    completionEvent.AthleteUserId,
                    completionEvent.EventType
                })
                .IsUnique();
            entity.HasIndex(completionEvent => new
                {
                    completionEvent.ProcessedAtUtc,
                    completionEvent.CreatedAtUtc
                });

            entity.ToTable(table => table.HasCheckConstraint(
                "CK_AssignmentCompletionEvents_EventType",
                "[EventType] = 'AssignmentRecipientCompleted'"));
            entity.ToTable(table => table.HasCheckConstraint(
                "CK_AssignmentCompletionEvents_ProcessingAttempts",
                "[ProcessingAttempts] >= 0"));

            entity.HasOne(completionEvent => completionEvent.Assignment)
                .WithMany()
                .HasForeignKey(completionEvent => completionEvent.AssignmentId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(completionEvent => completionEvent.AthleteUser)
                .WithMany()
                .HasForeignKey(completionEvent => completionEvent.AthleteUserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(completionEvent => completionEvent.Drill)
                .WithMany()
                .HasForeignKey(completionEvent => completionEvent.DrillId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AthleteProgression>(entity =>
        {
            entity.HasOne(progress => progress.AthleteUser).WithMany()
                .HasForeignKey(progress => progress.AthleteUserId).OnDelete(DeleteBehavior.NoAction);
            entity.ToTable(table =>
            {
                table.HasCheckConstraint("CK_AthleteProgressions_Rank", "[OverallRank] BETWEEN 1 AND 8");
                table.HasCheckConstraint("CK_AthleteProgressions_NonNegative", "[ProgressionScore] >= 0 AND [TotalQualifyingCompletions] >= 0 AND [ActiveSkillCount] >= 0 AND [CurrentOverallStreak] >= 0 AND [LongestOverallStreak] >= 0");
                table.HasCheckConstraint("CK_AthleteProgressions_Percent", "[ProgressToNextRank] BETWEEN 0 AND 100");
            });
        });

        modelBuilder.Entity<AthleteSkillProgress>(entity =>
        {
            entity.HasIndex(progress => new { progress.AthleteUserId, progress.Sport, progress.Category, progress.SubCategory }).IsUnique();
            entity.HasOne(progress => progress.AthleteUser).WithMany()
                .HasForeignKey(progress => progress.AthleteUserId).OnDelete(DeleteBehavior.NoAction);
            entity.ToTable(table =>
            {
                table.HasCheckConstraint("CK_AthleteSkillProgress_Level", "[CurrentLevel] BETWEEN 1 AND 5");
                table.HasCheckConstraint("CK_AthleteSkillProgress_NonNegative", "[QualifyingCompletions] >= 0 AND [CurrentStreak] >= 0 AND [LongestStreak] >= 0");
                table.HasCheckConstraint("CK_AthleteSkillProgress_Percent", "[ProgressToNextLevel] BETWEEN 0 AND 100");
                table.HasCheckConstraint("CK_AthleteSkillProgress_Rating", "[AverageRating] IS NULL OR ([AverageRating] >= 1 AND [AverageRating] <= 5)");
            });
        });

        modelBuilder.Entity<AthleteRankHistory>(entity =>
        {
            entity.HasIndex(x => new { x.AthleteUserId, x.RankNumber }).IsUnique();
            entity.HasIndex(x => new { x.AthleteUserId, x.EarnedAtUtc });
            entity.HasOne(x => x.AthleteUser).WithMany().HasForeignKey(x => x.AthleteUserId).OnDelete(DeleteBehavior.NoAction);
            entity.ToTable(t => t.HasCheckConstraint("CK_AthleteRankHistories_Rank", "[RankNumber] BETWEEN 2 AND 8"));
        });
        modelBuilder.Entity<AthleteSkillLevelHistory>(entity =>
        {
            entity.HasIndex(x => new { x.AthleteUserId, x.Sport, x.Category, x.SubCategory, x.Level }).IsUnique();
            entity.HasIndex(x => new { x.AthleteUserId, x.EarnedAtUtc });
            entity.HasOne(x => x.AthleteUser).WithMany().HasForeignKey(x => x.AthleteUserId).OnDelete(DeleteBehavior.NoAction);
            entity.ToTable(t => t.HasCheckConstraint("CK_AthleteSkillLevelHistories_Level", "[Level] BETWEEN 2 AND 5"));
        });
        modelBuilder.Entity<AchievementDefinition>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.ToTable(t => t.HasCheckConstraint("CK_AchievementDefinitions_SortOrder", "[SortOrder] >= 0"));
        });
        modelBuilder.Entity<AthleteAchievement>(entity =>
        {
            entity.HasIndex(x => new { x.AthleteUserId, x.AchievementDefinitionId }).IsUnique();
            entity.HasIndex(x => new { x.AthleteUserId, x.EarnedAtUtc });
            entity.HasOne(x => x.AthleteUser).WithMany().HasForeignKey(x => x.AthleteUserId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.AchievementDefinition).WithMany().HasForeignKey(x => x.AchievementDefinitionId).OnDelete(DeleteBehavior.NoAction);
        });
        modelBuilder.Entity<AthleteGoal>(entity =>
        {
            entity.HasIndex(x => new { x.AthleteUserId, x.Status, x.CreatedAtUtc });
            entity.HasOne(x => x.AthleteUser).WithMany().HasForeignKey(x => x.AthleteUserId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.NoAction);
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_AthleteGoals_TargetValue", "[TargetValue] > 0");
                t.HasCheckConstraint("CK_AthleteGoals_GoalType", "[GoalType] IN ('QualifyingCompletions','SkillLevel','OverallRank','TrainingStreak')");
                t.HasCheckConstraint("CK_AthleteGoals_Status", "[Status] IN ('Active','Completed','Cancelled')");
            });
        });
        modelBuilder.Entity<TrainingRequest>(entity =>
        {
            entity.HasIndex(x => new { x.AthleteUserId, x.Status, x.CreatedAtUtc });
            entity.HasIndex(x => new { x.RequestedRecipientUserId, x.Status, x.CreatedAtUtc });
            entity.HasIndex(x => x.ApprovedAssignmentId).IsUnique().HasFilter("[ApprovedAssignmentId] IS NOT NULL");
            entity.HasOne(x => x.AthleteUser).WithMany().HasForeignKey(x => x.AthleteUserId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.RequestedRecipientUser).WithMany().HasForeignKey(x => x.RequestedRecipientUserId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Team).WithMany().HasForeignKey(x => x.TeamId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.RequestedDrill).WithMany().HasForeignKey(x => x.RequestedDrillId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.ApprovedAssignment).WithMany().HasForeignKey(x => x.ApprovedAssignmentId).OnDelete(DeleteBehavior.NoAction);
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_TrainingRequests_Status", "[Status] IN ('Pending','Approved','Declined','Cancelled')");
                t.HasCheckConstraint("CK_TrainingRequests_RecipientRole", "[RequestedRecipientRole] IN ('Parent','Coach')");
            });
        });
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(x=>new{x.RecipientUserId,x.IsRead,x.CreatedAtUtc});
            entity.HasIndex(x=>new{x.RecipientUserId,x.CreatedAtUtc});
            entity.HasIndex(x=>new{x.RecipientUserId,x.Type,x.SourceKey}).IsUnique();
            entity.HasOne(x=>x.RecipientUser).WithMany().HasForeignKey(x=>x.RecipientUserId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x=>x.ActorUser).WithMany().HasForeignKey(x=>x.ActorUserId).OnDelete(DeleteBehavior.NoAction);
        });
        modelBuilder.Entity<NotificationEvent>(entity =>
        {
            entity.HasIndex(x=>new{x.ProcessedAtUtc,x.ProcessingAttempts,x.CreatedAtUtc});
            entity.HasIndex(x=>new{x.EventType,x.SourceKey,x.RecipientUserId}).IsUnique();
            entity.HasOne(x=>x.RecipientUser).WithMany().HasForeignKey(x=>x.RecipientUserId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x=>x.ActorUser).WithMany().HasForeignKey(x=>x.ActorUserId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x=>x.SubjectUser).WithMany().HasForeignKey(x=>x.SubjectUserId).OnDelete(DeleteBehavior.NoAction);
            entity.ToTable(t=>t.HasCheckConstraint("CK_NotificationEvents_ProcessingAttempts","[ProcessingAttempts] >= 0"));
        });
    }

    public static List<Drill> GetHardcodedDrills()
    {
        return new List<Drill>();
    }
}
