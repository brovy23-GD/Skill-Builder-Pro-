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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .ToTable("Users");

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(user => user.Profile)
            .WithOne(profile => profile.User)
            .HasForeignKey<UserProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProgressLog>()
            .HasOne(log => log.Owner)
            .WithMany()
            .HasForeignKey(log => log.OwnerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<TrainingSchedule>()
            .HasOne(schedule => schedule.Owner)
            .WithMany()
            .HasForeignKey(schedule => schedule.OwnerUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    public static List<Drill> GetHardcodedDrills()
    {
        return new List<Drill>();
    }
}
