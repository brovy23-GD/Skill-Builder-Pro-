using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Drill> Drills => Set<Drill>();
    public DbSet<TrainingSchedule> Schedules => Set<TrainingSchedule>();
    public DbSet<ProgressLog> ProgressLogs => Set<ProgressLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrainingSchedule>()
            .HasOne(s => s.Drill)
            .WithMany(d => d.Schedules)
            .HasForeignKey(s => s.DrillId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProgressLog>()
            .HasOne(p => p.Drill)
            .WithMany(d => d.ProgressLogs)
            .HasForeignKey(p => p.DrillId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed data
        modelBuilder.Entity<Drill>().HasData(
            new Drill { Id = 1, Name = "Stationary Ball Handling", Sport = "Basketball", Category = "Dribbling", Description = "Two-ball pound dribbles, crossovers, figure 8s.", DifficultyLevel = 2 },
            new Drill { Id = 2, Name = "Tee Work - Inside Pitch", Sport = "Softball", Category = "Hitting", Description = "Tee placement inside, focus on staying connected through the zone.", DifficultyLevel = 2 },
            new Drill { Id = 3, Name = "Form Shooting Close Range", Sport = "Basketball", Category = "Shooting", Description = "One-hand form shots from 5 spots inside the paint.", DifficultyLevel = 1 }
        );
    }
}
